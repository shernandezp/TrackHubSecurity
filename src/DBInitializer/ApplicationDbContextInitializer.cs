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
        if (!context.Resources.Any())
        {
            context.Resources.Add(new Resource { ResourceName = Resources.Accounts });
            context.Resources.Add(new Resource { ResourceName = Resources.Administrative });
            context.Resources.Add(new Resource { ResourceName = Resources.Credentials });
            context.Resources.Add(new Resource { ResourceName = Resources.Devices });
            context.Resources.Add(new Resource { ResourceName = Resources.Geofences });
            context.Resources.Add(new Resource { ResourceName = Resources.Geofencing });
            context.Resources.Add(new Resource { ResourceName = Resources.Groups });
            context.Resources.Add(new Resource { ResourceName = Resources.Operators });
            context.Resources.Add(new Resource { ResourceName = Resources.Permissions });
            context.Resources.Add(new Resource { ResourceName = Resources.Positions });
            context.Resources.Add(new Resource { ResourceName = Resources.Profile });
            context.Resources.Add(new Resource { ResourceName = Resources.Reports });
            context.Resources.Add(new Resource { ResourceName = Resources.SettingsScreen });
            context.Resources.Add(new Resource { ResourceName = Resources.Transporters });
            context.Resources.Add(new Resource { ResourceName = Resources.TransporterType });
            context.Resources.Add(new Resource { ResourceName = Resources.Users });
            await context.SaveChangesAsync();
        }
        if (!context.Actions.Any())
        {
            context.Actions.Add(new Action { ActionName = Actions.Read });
            context.Actions.Add(new Action { ActionName = Actions.Edit });
            context.Actions.Add(new Action { ActionName = Actions.Export });
            context.Actions.Add(new Action { ActionName = Actions.Execute });
            context.Actions.Add(new Action { ActionName = Actions.Write });
            context.Actions.Add(new Action { ActionName = Actions.Delete });
            context.Actions.Add(new Action { ActionName = Actions.Custom });
            await context.SaveChangesAsync();
        }
        if (!context.ResourceActions.Any())
        {
            for (int resource = 1; resource <= 16; resource++)
            {
                for (int action = 1; action <= 6; action++)
                {
                    context.ResourceActions.Add(new ResourceAction { ResourceId = resource, ActionId = action });
                }
            }
            var userResource = await context.Resources.FirstAsync(x => x.ResourceName == Resources.Users);
            var passwordAction = await context.Actions.FirstAsync(x => x.ActionName == Actions.Custom);
            context.ResourceActions.Add(new ResourceAction { ResourceId = userResource.ResourceId, ActionId = passwordAction.ActionId });

            await context.SaveChangesAsync();
        }
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
            for (int resource = 1; resource <= 14; resource++)
            {
                for (int action = 1; action <= 6; action++)
                {
                    context.ResourceActionRole.Add(new ResourceActionRole { ResourceId = resource, ActionId = action, RoleId = 1 });
                }
            }
            await context.SaveChangesAsync();
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
            var password = "123456".HashPassword();
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
}
