// Copyright (c) 2024 Sergio Hernandez. All rights reserved.
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
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Action = TrackHub.Security.Infrastructure.SecurityDB.Entities.Action;

namespace TrackHub.Security.Infrastructure.SecurityDB;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
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
            context.Resources.Add(new Resource { ResourceName = Resources.AccountScreen });
            context.Resources.Add(new Resource { ResourceName = Resources.MapScreen });
            context.Resources.Add(new Resource { ResourceName = Resources.PermissionScreen });
            context.Resources.Add(new Resource { ResourceName = Resources.SettingsScreen });
            context.Resources.Add(new Resource { ResourceName = Resources.UserScreen });
            await context.SaveChangesAsync();
        }
        if (!context.Actions.Any())
        {
            context.Actions.Add(new Action { ActionName = Actions.View, ResourceId = 1 });
            context.Actions.Add(new Action { ActionName = Actions.Edit, ResourceId = 1 });
            context.Actions.Add(new Action { ActionName = Actions.Export, ResourceId = 1 });
            context.Actions.Add(new Action { ActionName = Actions.Execute, ResourceId = 1 });
            context.Actions.Add(new Action { ActionName = Actions.Write, ResourceId = 1 });
            await context.SaveChangesAsync();
        }
        if (!context.Roles.Any())
        {
            context.Roles.Add(new Role { RoleName = "Manager", Description = string.Empty });
            context.Roles.Add(new Role { RoleName = "User", Description = string.Empty });
            await context.SaveChangesAsync();
        }
        if (!context.Policies.Any())
        {
            context.Policies.Add(new Policy { PolicyName = "CanView", Description = string.Empty });
            context.Policies.Add(new Policy { PolicyName = "CanEdit", Description = string.Empty });
            context.Policies.Add(new Policy { PolicyName = "CanExport", Description = string.Empty });
            context.Policies.Add(new Policy { PolicyName = "CanExecute", Description = string.Empty });
            context.Policies.Add(new Policy { PolicyName = "CanWrite", Description = string.Empty });
            await context.SaveChangesAsync();
        }
        if (!context.ResourceActionRole.Any())
        {
            context.ResourceActionRole.Add(new ResourceActionRole { ResourceId = 1, ActionId = 1, RoleId = 2 });    //View Map: User
            context.ResourceActionRole.Add(new ResourceActionRole { ResourceId = 1, ActionId = 1, RoleId = 1 });    //View Map: Manager
            context.ResourceActionRole.Add(new ResourceActionRole { ResourceId = 1, ActionId = 2, RoleId = 1 });    //Edit Map: Manager
            context.ResourceActionRole.Add(new ResourceActionRole { ResourceId = 1, ActionId = 3, RoleId = 1 });    //Export Map: Manager
            context.ResourceActionRole.Add(new ResourceActionRole { ResourceId = 1, ActionId = 3, RoleId = 2 });    //Export Map: User
            await context.SaveChangesAsync();
        }
        if (!context.ResourceActionPolicy.Any())
        {
            context.ResourceActionPolicy.Add(new ResourceActionPolicy { ResourceId = 1, ActionId = 1, PolicyId = 1 });  //View Map: CanView
            context.ResourceActionPolicy.Add(new ResourceActionPolicy { ResourceId = 1, ActionId = 2, PolicyId = 2 });  //Edit Map: CanEdit
            context.ResourceActionPolicy.Add(new ResourceActionPolicy { ResourceId = 1, ActionId = 3, PolicyId = 3 });  //Export Map: CanExport
            await context.SaveChangesAsync();
        }

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
                Guid.NewGuid()));

            await context.SaveChangesAsync();
        }
    }
}
