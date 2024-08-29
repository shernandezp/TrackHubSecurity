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
            context.Resources.Add(new Resource { ResourceName = Resources.Accounts });
            context.Resources.Add(new Resource { ResourceName = Resources.Positions });
            context.Resources.Add(new Resource { ResourceName = Resources.Permissions });
            context.Resources.Add(new Resource { ResourceName = Resources.SettingsScreen });
            context.Resources.Add(new Resource { ResourceName = Resources.Users });
            context.Resources.Add(new Resource { ResourceName = Resources.Credentials });
            context.Resources.Add(new Resource { ResourceName = Resources.Devices });
            context.Resources.Add(new Resource { ResourceName = Resources.Operators });
            context.Resources.Add(new Resource { ResourceName = Resources.Transporters });
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
            context.Actions.Add(new Action { ActionName = Actions.UpdatePassword });
            context.Actions.Add(new Action { ActionName = Actions.RefreshToken });
            context.Actions.Add(new Action { ActionName = Actions.ConnectivityTest });
            await context.SaveChangesAsync();
        }
        if (!context.ResourceActions.Any())
        {
            for (int resource = 1; resource <= 9; resource++)
            {
                for (int action = 1; action <= 6; action++)
                {
                    context.ResourceActions.Add(new ResourceAction { ResourceId = resource, ActionId = action });
                }
            }
            await context.SaveChangesAsync();
        }
        if (!context.Roles.Any())
        {
            context.Roles.Add(new Role { Name = "Admin", Description = string.Empty });
            context.Roles.Add(new Role { Name = "Manager", Description = string.Empty });
            context.Roles.Add(new Role { Name = "User", Description = string.Empty });
            await context.SaveChangesAsync();
        }
        
        if (!context.Policies.Any())
        {
            context.Policies.Add(new Policy { Name = "CanView", Description = string.Empty });
            context.Policies.Add(new Policy { Name = "CanEdit", Description = string.Empty });
            context.Policies.Add(new Policy { Name = "CanExport", Description = string.Empty });
            context.Policies.Add(new Policy { Name = "CanExecute", Description = string.Empty });
            context.Policies.Add(new Policy { Name = "CanWrite", Description = string.Empty });
            context.Policies.Add(new Policy { Name = "CanDelete", Description = string.Empty });
            await context.SaveChangesAsync();
        }
        if (!context.ResourceActionRole.Any())
        {
            for (int resource = 1; resource <= 9; resource++)
            {
                for (int action = 1; action <= 6; action++)
                {
                    context.ResourceActionRole.Add(new ResourceActionRole { ResourceId = resource, ActionId = action, RoleId = 1 });
                }
            }
            await context.SaveChangesAsync();
        }
        if (!context.ResourceActionPolicy.Any())
        {
            for (int resource = 1; resource <= 9; resource++)
            {
                for (int action = 1; action <= 6; action++)
                {
                    context.ResourceActionPolicy.Add(new ResourceActionPolicy { ResourceId = resource, ActionId = action, PolicyId = 1 });
                    context.ResourceActionPolicy.Add(new ResourceActionPolicy { ResourceId = resource, ActionId = action, PolicyId = 2 });
                    context.ResourceActionPolicy.Add(new ResourceActionPolicy { ResourceId = resource, ActionId = action, PolicyId = 3 });
                }
            }
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
                true,
                0,
                Guid.NewGuid()));

            await context.SaveChangesAsync();
        }
    }
}
