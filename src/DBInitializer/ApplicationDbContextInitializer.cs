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
using TrackHub.Security.Infrastructure.SecurityDB.Entities;
using TrackHub.Security.Infrastructure.SecurityDB;
using Microsoft.EntityFrameworkCore;
using Action = TrackHub.Security.Infrastructure.SecurityDB.Entities.Action;

namespace DBInitializer;
internal class ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context)
{
    private static readonly string[] DefaultResources =
    [
        Resources.Accounts,
        Resources.AccountsMaster,
        Resources.Administrative,
        Resources.AccountFeatures,
        Resources.Alerts,
        Resources.Audit,
        Resources.BackgroundJobs,
        Resources.Credentials,
        Resources.Devices,
        Resources.DevicesMaster,
        Resources.Documents,
        Resources.Drivers,
        Resources.Geofences,
        Resources.Geofencing,
        Resources.Groups,
        Resources.ManageDevices,
        Resources.Notifications,
        Resources.Operators,
        Resources.OperatorsMaster,
        Resources.Permissions,
        Resources.Positions,
        Resources.Profile,
        Resources.PublicLinks,
        Resources.Reports,
        Resources.SettingsScreen,
        Resources.ServiceClients,
        Resources.SupportGrants,
        Resources.Transporters,
        Resources.TransporterType,
        Resources.Users
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

    public async Task InitializeAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

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
        foreach (var resourceName in new[] { Resources.Users, Resources.Positions, Resources.Credentials })
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
            context.Roles.Add(new Role { Name = Roles.Administrator, Description = string.Empty });
            context.Roles.Add(new Role { Name = Roles.Manager, Description = string.Empty, ParentRoleId = 1 });
            context.Roles.Add(new Role { Name = Roles.User, Description = string.Empty, ParentRoleId = 2 });
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

        if (!context.ResourceActionRole.Any())
        {
            await SeedAdministratorResourceActionsAsync();
        }
        else
        {
            await SeedAdministratorResourceActionsAsync();
        }

        /*if (!context.ResourceActionPolicy.Any())
        {
            for (int resource = 1; resource <= 10; resource++)
            {
                for (int action = 1; action <= 5; action++)
                {
                    context.ResourceActionPolicy.Add(new ResourceActionPolicy { ResourceId = resource, ActionId = action, PolicyId = 1 });
                    context.ResourceActionPolicy.Add(new ResourceActionPolicy { ResourceId = resource, ActionId = action, PolicyId = 2 });
                    context.ResourceActionPolicy.Add(new ResourceActionPolicy { ResourceId = resource, ActionId = action, PolicyId = 3 });
                }
            }
            await context.SaveChangesAsync();
        }*/
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
        //TODO: Remove this after the first run, only to update the password of the default user
        else
        {
            var user = context.Users.First(x => x.Username == "Administrator");
            context.Users.Attach(user);
            var password = "12345678".HashPassword();
            user.Password = password;
            await context.SaveChangesAsync();
        }
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
