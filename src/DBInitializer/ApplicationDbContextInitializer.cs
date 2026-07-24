// Copyright (c) 2025 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using Common.Domain.Constants;
using Common.Domain.Extensions;
using Microsoft.Extensions.Logging;
using TrackHub.Security.Infrastructure.Entities;
using TrackHub.Security.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Action = TrackHub.Security.Infrastructure.Entities.Action;

namespace DBInitializer;
internal class ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context)
{
    private static readonly string[] DefaultResources =
    [
        Resources.Accounts,
        Resources.AccountsMaster,
        Resources.Administrative,
        Resources.AccountFeatures,
        Resources.AccountFeaturesMaster,
        Resources.Alerts,
        Resources.Audit,
        Resources.BackgroundJobs,
        Resources.Credentials,
        Resources.Devices,
        Resources.DevicesMaster,
        Resources.Documents,
        Resources.Drivers,
        Resources.GeocodingProviders,
        Resources.Geofences,
        Resources.Geofencing,
        Resources.GpsIntegrationDashboard,
        Resources.Groups,
        Resources.Notifications,
        Resources.Operators,
        Resources.OperatorHealth,
        Resources.OperatorSyncRuns,
        Resources.OperatorsMaster,
        Resources.Permissions,
        Resources.PointsOfInterest,
        Resources.Positions,
        Resources.PositionHistory,
        Resources.Profile,
        Resources.PublicLinks,
        Resources.Reports,
        Resources.ServiceClients,
        Resources.SupportGrants,
        Resources.SynchronizedDevices,
        Resources.TollCatalog,
        Resources.Transporters,
        Resources.TransporterType,
        Resources.Trips,
        Resources.TripTracking,
        Resources.Users,
    ];

    private static readonly string[] DefaultActions =
    [
        Actions.Read,
        Actions.Edit,
        Actions.Export,
        Actions.Execute,
        Actions.Write,
        Actions.Delete
    ];

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default data
        // Seed, if necessary
        foreach (var resourceName in DefaultResources)
        {
            if (!await context.Resources.AnyAsync(x => x.ResourceName == resourceName))
            {
                context.Resources.Add(new Resource { ResourceName = resourceName });
            }
        }

        await context.SaveChangesAsync();

        foreach (var actionName in DefaultActions.Append(Actions.Custom))
        {
            if (!await context.Actions.AnyAsync(x => x.ActionName == actionName))
            {
                context.Actions.Add(new Action { ActionName = actionName });
            }
        }

        await context.SaveChangesAsync();

        var resources = await context.Resources.ToListAsync();
        var actions = await context.Actions.ToListAsync();
        var standardActions = actions.Where(x => DefaultActions.Contains(x.ActionName)).ToList();

        foreach (var resource in resources)
        {
            foreach (var action in standardActions)
            {
                if (!await context.ResourceActions.AnyAsync(x => x.ResourceId == resource.ResourceId && x.ActionId == action.ActionId))
                {
                    context.ResourceActions.Add(new ResourceAction { ResourceId = resource.ResourceId, ActionId = action.ActionId });
                }
            }
        }

        var customAction = await context.Actions.FirstAsync(x => x.ActionName == Actions.Custom);
        // Resources.Trips needs Custom because spec 11's planTripRoute, shareTrip and
        // revokeTripShare are user-facing Custom operations. Without a ResourceAction row here
        // there is no ResourceActionRole to grant, so IdentityService resolves no roles and
        // AuthorizationBehavior returns FORBIDDEN for EVERY user including Administrator —
        // route planning and the whole public-sharing surface would be dead on a fresh deploy.
        // (TripTracking/Custom is deliberately NOT here: only service clients call it, and they
        // authorize through the separate service_client_permissions table.)
        foreach (var resourceName in new[] { Resources.Users, Resources.Positions, Resources.Credentials, Resources.Notifications, Resources.Trips, Resources.Operators })
        {
            var resource = await context.Resources.FirstAsync(x => x.ResourceName == resourceName);
            if (!await context.ResourceActions.AnyAsync(x => x.ResourceId == resource.ResourceId && x.ActionId == customAction.ActionId))
            {
                context.ResourceActions.Add(new ResourceAction { ResourceId = resource.ResourceId, ActionId = customAction.ActionId });
            }
        }

        await context.SaveChangesAsync();
        if (!context.Roles.Any())
        {
            // Seed the parents first, then resolve their generated ids by name so the
            // Administrator -> Manager -> User hierarchy is wired up without hardcoding row ids.
            context.Roles.Add(new Role { Name = Roles.Administrator, Description = string.Empty });
            await context.SaveChangesAsync();

            var administratorRole = await context.Roles.FirstAsync(x => x.Name == Roles.Administrator);
            context.Roles.Add(new Role { Name = Roles.Manager, Description = string.Empty, ParentRoleId = administratorRole.RoleId });
            await context.SaveChangesAsync();

            var managerRole = await context.Roles.FirstAsync(x => x.Name == Roles.Manager);
            context.Roles.Add(new Role { Name = Roles.User, Description = string.Empty, ParentRoleId = managerRole.RoleId });
            await context.SaveChangesAsync();
        }

        if (!context.Policies.Any())
        {
            context.Policies.Add(new Policy { Name = "FullAccess", Description = string.Empty });
            context.Policies.Add(new Policy { Name = "ManageUsers", Description = string.Empty });
            context.Policies.Add(new Policy { Name = "ReadOnly", Description = string.Empty });
            context.Policies.Add(new Policy { Name = "LimitedUpdate", Description = string.Empty });
            context.Policies.Add(new Policy { Name = "Audit", Description = string.Empty });
            await context.SaveChangesAsync();
        }

        // Administrator keeps grant-all (every catalogued resource/action pair).
        await SeedAdministratorResourceActionsAsync();

        // Data-driven default role matrix for the non-admin roles. Each entry is the full set of
        // resource -> actions a role is granted; SeedRoleResourceActionsAsync is idempotent and
        // silently skips any resource/action pair that isn't in the ResourceAction catalog, so
        // resources not listed for a role receive no grant. Administrator is intentionally absent
        // here — it is handled by SeedAdministratorResourceActionsAsync above.
        //
        // Rationale anchors: Geofences full CRUD for User (portal Geofence Manager route is
        // USER-gated); Positions is Read-only for both roles (Positions/Custom is a service-identity
        // write from the Router feed, with no portal or mobile caller); Operators/Custom is ping and
        // manual sync, held by Manager and User; Users/Custom is SELF-SERVICE password change only
        // (UpdatePasswordCommand re-checks that the subject is the caller);
        // Credentials (viewing decrypted credential material) and Drivers are Manager-only;
        // Notifications/Custom (SendTest) is Manager-only;
        // Reports/Edit, SupportGrants, ServiceClients, Administrative, *Master and GeocodingProviders
        // stay Administrator-only.
        //
        // Trip Management (spec 11 §15): Manager gets Trips R/W/E/D/Export/Custom + TollCatalog/Read;
        // User gets Trips R/W/E/Export/Custom + TollCatalog/Read. TollCatalog Write/Edit/Delete
        // is deliberately NOT granted to Manager or User — the toll catalog is platform reference
        // data, so a non-administrator can read stations/tariffs/vehicle classes but can never
        // create, edit or delete one (spec 11 §17 acceptance 5).
        //
        // TripTracking carries NO role grant at all. §15 called for Manager → TripTracking/Read,
        // but the resource has exactly one operation — processTripPositions, which is
        // TripTracking/Custom — so a Read grant authorized nothing while standing as a live
        // permission row that would silently pre-authorize any TripTracking read added later.
        // TripTracking/Custom is a service-identity grant only and is seeded below.
        //
        // NOTE: SeedRoleResourceActionsAsync only ADDS missing rows, it never removes. A deployment
        // already seeded with the earlier matrix keeps its inert Manager → TripTracking/Read row;
        // delete it manually if you want the permission table to match this list exactly.
        var roleMatrix = new Dictionary<string, (string Resource, string[] Actions)[]>
        {
            [Roles.Manager] =
            [
                (Resources.Accounts, [Actions.Read, Actions.Edit]),
                (Resources.AccountFeatures, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete]),
                (Resources.Alerts, [Actions.Read, Actions.Edit]),
                (Resources.Audit, [Actions.Read]),
                (Resources.BackgroundJobs, [Actions.Read]),
                (Resources.Credentials, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete]),
                (Resources.Devices, [Actions.Read, Actions.Delete]),
                (Resources.Documents, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete]),
                (Resources.Drivers, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete]),
                (Resources.Geofences, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete]),
                (Resources.Geofencing, [Actions.Read]),
                (Resources.GpsIntegrationDashboard, [Actions.Read]),
                (Resources.Groups, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete]),
                (Resources.Notifications, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete, Actions.Custom]),
                // Custom = operate the GPS integration (pingOperator, triggerOperatorSync). The Router
                // reaches the provider with its own service identity, so this grants operation of the
                // integration without granting sight of credential material.
                (Resources.Operators, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete, Actions.Custom]),
                (Resources.OperatorHealth, [Actions.Read]),
                (Resources.OperatorSyncRuns, [Actions.Read]),
                (Resources.Permissions, [Actions.Read]),
                (Resources.PointsOfInterest, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete]),
                // Read only. Positions/Custom gates two commands — bulkTransporterPosition and
                // persistResolvedAddress — and both are service-identity writes from the Router feed.
                // Every user-facing positions operation uses Actions.Read.
                (Resources.Positions, [Actions.Read]),
                (Resources.PositionHistory, [Actions.Read]),
                (Resources.Profile, [Actions.Read, Actions.Edit]),
                (Resources.PublicLinks, [Actions.Read, Actions.Write, Actions.Delete]),
                (Resources.Reports, [Actions.Read]),
                (Resources.SynchronizedDevices, [Actions.Read, Actions.Write, Actions.Edit, Actions.Execute]),
                (Resources.TollCatalog, [Actions.Read]),
                (Resources.Transporters, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete]),
                (Resources.TransporterType, [Actions.Read]),
                // Custom is planTripRoute / shareTrip / revokeTripShare. Adding Trips to the
                // Actions.Custom resource list only makes the pair GRANTABLE — authorization reads
                // ResourceActionRole, and role inheritance does not fill the gap
                // (ResourceActionRoleReader matches role names exactly, no ParentRoleId walk).
                // Without it an account administrator could create a trip but not plan its route
                // or share it, and only the platform Administrator's grant-all covered it.
                (Resources.Trips, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete, Actions.Export, Actions.Custom]),
                (Resources.Users, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete, Actions.Custom]),
            ],
            [Roles.User] =
            [
                (Resources.Accounts, [Actions.Read]),
                (Resources.Alerts, [Actions.Read]),
                (Resources.Devices, [Actions.Read]),
                (Resources.Documents, [Actions.Read]),
                (Resources.Geofences, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete]),
                (Resources.Geofencing, [Actions.Read]),
                (Resources.Groups, [Actions.Read]),
                (Resources.Notifications, [Actions.Read, Actions.Write, Actions.Edit, Actions.Delete]),
                // Custom = ping / manual sync. See the Manager entry.
                (Resources.Operators, [Actions.Read, Actions.Custom]),
                (Resources.PointsOfInterest, [Actions.Read]),
                // Read only — see the note on the Manager entry above.
                (Resources.Positions, [Actions.Read]),
                (Resources.PositionHistory, [Actions.Read]),
                (Resources.Profile, [Actions.Read, Actions.Edit]),
                (Resources.Reports, [Actions.Read]),
                (Resources.TollCatalog, [Actions.Read]),
                (Resources.Transporters, [Actions.Read]),
                (Resources.TransporterType, [Actions.Read]),
                // Custom = plan route / share / revoke. The dispatcher is the actor spec 11 §4 names
                // for exactly these three operations, so withholding it left the primary user of
                // the module unable to plan a route on a trip they had just created.
                // Export = the six trip-* report feeds, including the POD register. Without it the
                // dispatcher-facing catalog reports were visible in the picker but failed closed for
                // the role that owns them.
                (Resources.Trips, [Actions.Read, Actions.Write, Actions.Edit, Actions.Export, Actions.Custom]),
                // Custom on Users gates exactly one command: UpdatePasswordCommand, whose handler
                // requires the subject to be the caller (or a verified manager of the subject). This
                // is the self-service password change; it grants no user administration.
                (Resources.Users, [Actions.Custom]),
            ],
        };

        foreach (var (roleName, grants) in roleMatrix)
        {
            foreach (var (resource, actionNames) in grants)
            {
                await SeedRoleResourceActionsAsync(roleName, resource, actionNames);
            }
        }

        // Service-client allowlist for the TrackHub.Telemetry surface. The token
        // audience is the shared trackhub_api, so that is the audience these grants match at
        // enforcement time. Removing a row blocks the corresponding operation with FORBIDDEN.
        await SeedTelemetryServiceClientPermissionsAsync();

        // Service-client identity + Manager-surface allowlist: IsValidServiceAsync requires the
        // client NAME to exist in security.clients before any permission row is even consulted,
        // and the Router/SyncWorker service flows hit Manager master-data operations that the
        // telemetry seeding above does not cover.
        await SeedServiceClientRegistrationsAsync();
        await SeedManagerServiceClientPermissionsAsync();

        // Security's own service identity: forwards security audit events to Manager's AuditEvent store.
        await SeedSecurityServiceClientPermissionsAsync();

        // Geofencing's service identity: emits geofence alert events into Manager's alert
        // pipeline and records its dwell-evaluator job runs there.
        await SeedGeofenceServiceClientPermissionsAsync();

        // Trip Management's service identity: emits trip alert events and job runs, reads the
        // Manager/Telemetry master data route planning and tracking need, and manages the public
        // tracking links. The Router/SyncWorker side additionally needs TripTracking/Custom to feed
        // the position pipeline (processTripPositions).
        // NOTE: db-init MUST be re-run on existing deployments after this change, or every trip
        // call returns FORBIDDEN — the resource catalog, role grants and service-client permission
        // rows above are what the authorization pipeline consults at enforcement time (spec 11 §15).
        await SeedTripServiceClientPermissionsAsync();

        if (!context.Users.Any())
        {
            var password = "12345678".HashPassword();
            context.Users.Add(new User(
                "Administrator",
                password,
                "email@mail.com",
                "Admin",
                "Admin",
                "",
                null,
                null,
                true,
                0,
                Guid.NewGuid()));

            await context.SaveChangesAsync();

            var user = context.Users.First();
            var admin = context.Roles.First(x => x.Name == Roles.Administrator);
            var manager = context.Roles.First(x => x.Name == Roles.Manager);
            context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = admin.RoleId });
            context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = manager.RoleId });

            await context.SaveChangesAsync();
        }
    }

    private async Task SeedRoleResourceActionsAsync(string roleName, string resourceName, string[] actionNames)
    {
        var role = await context.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
        var resource = await context.Resources.FirstOrDefaultAsync(x => x.ResourceName == resourceName);
        if (role is null || resource is null)
        {
            return;
        }

        foreach (var actionName in actionNames)
        {
            var action = await context.Actions.FirstOrDefaultAsync(x => x.ActionName == actionName);
            if (action is null)
            {
                continue;
            }

            if (!await context.ResourceActionRole.AnyAsync(x =>
                    x.RoleId == role.RoleId
                    && x.ResourceId == resource.ResourceId
                    && x.ActionId == action.ActionId))
            {
                context.ResourceActionRole.Add(new ResourceActionRole
                {
                    ResourceId = resource.ResourceId,
                    ActionId = action.ActionId,
                    RoleId = role.RoleId
                });
            }
        }

        await context.SaveChangesAsync();
    }

    // The two service identities every deployment needs. The real credential (secret) lives in
    // the AuthorityServer's OpenIddict application store; this row is the app-level allowlist
    // that IsValidClientAsync checks by NAME. Without it, every service-token call is denied.
    private async Task SeedServiceClientRegistrationsAsync()
    {
        string[] clients = ["router_client", "syncworker_client", "security_client", "geofence_client", "trip_client"];

        foreach (var name in clients)
        {
            if (await context.Clients.AnyAsync(c => c.Name == name))
            {
                continue;
            }

            context.Clients.Add(new Client(
                name,
                userId: null,
                description: "Service client (seeded; credential managed by the AuthorityServer)",
                secret: string.Empty,
                salt: string.Empty,
                processed: true));
        }

        await context.SaveChangesAsync();
    }

    // Manager master-data surface the Router/SyncWorker call with their SERVICE identity
    // (client credentials): account/operator/device-sync reads, device synchronization writes,
    // credential token refresh, alert recording, and the ServiceClient-only geocoding provider
    // read. Sourced from the [Authorize] attributes on the corresponding Manager handlers.
    private async Task SeedManagerServiceClientPermissionsAsync()
    {
        (string Resource, string Action)[] grants =
        [
            (Resources.Accounts, Actions.Read),
            (Resources.AccountsMaster, Actions.Read),
            (Resources.AccountFeatures, Actions.Read),
            (Resources.AccountFeaturesMaster, Actions.Read),
            (Resources.Operators, Actions.Read),
            (Resources.OperatorsMaster, Actions.Read),
            (Resources.SynchronizedDevices, Actions.Read),
            (Resources.SynchronizedDevices, Actions.Write),
            (Resources.Devices, Actions.Delete),
            (Resources.TransporterType, Actions.Read),
            (Resources.Credentials, Actions.Write),
            (Resources.Alerts, Actions.Write),
            (Resources.GeocodingProviders, Actions.Read),
        ];

        await SeedServiceClientPermissionsAsync(["router_client", "syncworker_client"], grants);
    }

    // The security_client posts security audit events to Manager's central AuditEvent store
    // It needs exactly one grant — Audit/Write — and nothing else.
    private async Task SeedSecurityServiceClientPermissionsAsync()
    {
        (string Resource, string Action)[] grants =
        [
            (Resources.Audit, Actions.Write),
        ];

        await SeedServiceClientPermissionsAsync(["security_client"], grants);
    }

    // The geofence_client records geofence alert events (recordAlertEvent) and its dwell
    // evaluator's job runs (createBackgroundJobRun) in Manager — exactly two grants.
    // The Router/SyncWorker side additionally needs Geofencing/Custom to feed the real-time
    // detection pipeline (processPositions) — without it every batch is denied and no geofence
    // events or alerts can ever be produced.
    private async Task SeedGeofenceServiceClientPermissionsAsync()
    {
        (string Resource, string Action)[] emitterGrants =
        [
            (Resources.Alerts, Actions.Write),
            (Resources.BackgroundJobs, Actions.Write),
        ];

        await SeedServiceClientPermissionsAsync(["geofence_client"], emitterGrants);

        (string Resource, string Action)[] detectionGrants =
        [
            (Resources.Geofencing, Actions.Custom),
        ];

        await SeedServiceClientPermissionsAsync(["router_client", "syncworker_client"], detectionGrants);
    }

    // The trip_client calls Manager (alert events, job runs, driver/transporter/group master data,
    // trip document metadata, public tracking links) and Telemetry (route replay position history)
    // with its SERVICE identity. Sourced from the [Authorize] attributes on the corresponding
    // producer handlers — this is the complete allowlist; removing a row blocks that operation with
    // FORBIDDEN. The Router/SyncWorker side gets TripTracking/Custom and NOTHING else in this
    // module, so those two identities can call processTripPositions only (spec 11 §17 acceptance 8).
    private async Task SeedTripServiceClientPermissionsAsync()
    {
        (string Resource, string Action)[] tripGrants =
        [
            (Resources.Alerts, Actions.Write),
            (Resources.BackgroundJobs, Actions.Write),
            (Resources.Drivers, Actions.Read),
            (Resources.Transporters, Actions.Read),
            (Resources.Groups, Actions.Read),
            (Resources.Documents, Actions.Read),
            (Resources.PublicLinks, Actions.Write),
            (Resources.PublicLinks, Actions.Delete),
            (Resources.PublicLinks, Actions.Read),
            (Resources.PositionHistory, Actions.Read),
            (Resources.AccountFeatures, Actions.Read),
        ];

        await SeedServiceClientPermissionsAsync(["trip_client"], tripGrants);

        (string Resource, string Action)[] trackingGrants =
        [
            (Resources.TripTracking, Actions.Custom),
        ];

        await SeedServiceClientPermissionsAsync(["router_client", "syncworker_client"], trackingGrants);
    }

    private async Task SeedTelemetryServiceClientPermissionsAsync()
    {
        (string Resource, string Action)[] grants =
        [
            (Resources.Positions, Actions.Custom),
            (Resources.PositionHistory, Actions.Write),
            (Resources.PositionHistory, Actions.Read),
            (Resources.OperatorHealth, Actions.Write),
            (Resources.OperatorSyncRuns, Actions.Write),
        ];

        await SeedServiceClientPermissionsAsync(["router_client", "syncworker_client"], grants);
    }

    // Every identity seeded here is a PLATFORM-INTERNAL service client: it runs with no account
    // claim and legitimately operates across every tenant (Router/SyncWorker feed positions for all
    // accounts; geofence/trip/security clients emit events and job runs for all accounts). That
    // reach used to be implicit — a NULL accountid silently matched any account. It is now
    // declared: allowCrossAccount = true. A PARTNER/tenant-bound client must be seeded WITHOUT this
    // flag and WITH an accountid, so its grant only matches a token carrying that same account.
    private async Task SeedServiceClientPermissionsAsync(
        string[] clients,
        (string Resource, string Action)[] grants,
        bool allowCrossAccount = true)
    {
        const string serviceScope = "service_scope";
        const string serviceAudience = "trackhub_api";

        foreach (var clientId in clients)
        {
            foreach (var (resource, action) in grants)
            {
                var existing = await context.ServiceClientPermissions.FirstOrDefaultAsync(p =>
                        p.ClientId == clientId
                        && p.Resource == resource
                        && p.Action == action
                        && p.Scope == serviceScope
                        && p.Audience == serviceAudience);

                if (existing is not null)
                {
                    // Upgrade path: rows seeded before the flag existed carry the default false.
                    // Re-running db-init restores the declared reach for the internal identities.
                    if (existing.AllowCrossAccount != allowCrossAccount)
                    {
                        existing.AllowCrossAccount = allowCrossAccount;
                    }

                    continue;
                }

                context.ServiceClientPermissions.Add(new ServiceClientPermission(
                    clientId,
                    accountId: null,
                    resource,
                    action,
                    serviceScope,
                    serviceAudience,
                    active: true,
                    allowCrossAccount: allowCrossAccount));
            }
        }

        await context.SaveChangesAsync();
    }

    private async Task SeedAdministratorResourceActionsAsync()
    {
        var administrator = await context.Roles.FirstAsync(x => x.Name == Roles.Administrator);
        var resourceActions = await context.ResourceActions.ToListAsync();

        foreach (var resourceAction in resourceActions)
        {
            if (!await context.ResourceActionRole.AnyAsync(x =>
                    x.RoleId == administrator.RoleId
                    && x.ResourceId == resourceAction.ResourceId
                    && x.ActionId == resourceAction.ActionId))
            {
                context.ResourceActionRole.Add(new ResourceActionRole
                {
                    ResourceId = resourceAction.ResourceId,
                    ActionId = resourceAction.ActionId,
                    RoleId = administrator.RoleId
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
