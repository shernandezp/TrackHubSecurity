﻿// Copyright (c) 2024 Sergio Hernandez. All rights reserved.
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

using TrackHub.Security.Infrastructure.SecurityDB.Interfaces;

namespace TrackHub.Security.Infrastructure.SecurityDB.Readers;


public sealed class ResourceActionRoleReader(IApplicationDbContext context) : IResourceActionRoleReader
{
    // Retrieves the roles associated with a specific resource and action
    public async Task<IReadOnlyCollection<string>> GetResourceActionRolesAsync(string resource, string action, CancellationToken cancellationToken)
        => await context.ResourceActionRole
            .Include(rar => rar.Resource)
            .Include(rar => rar.Action)
            .Include(rar => rar.Role)
            .Where(rar => rar.Resource.ResourceName.Equals(resource) && rar.Action.ActionName.Equals(action))
            .Select(rar => rar.Role.Name)
            .ToListAsync(cancellationToken);

    // Retrieves the authorized actions for a collection of roles
    public async Task<IReadOnlyCollection<ResourceActionVm>> GetRoleAuthorizedActionsAsync(IReadOnlyCollection<int> roles, CancellationToken cancellationToken)
        => await context.ResourceActionRole
            .Include(rar => rar.Resource)
            .Include(rar => rar.Action)
            .Where(rar => roles.Contains(rar.RoleId))
            .Select(rar => new ResourceActionVm(
                rar.ResourceId,
                rar.Resource.ResourceName,
                rar.ActionId,
                rar.Action.ActionName
            ))
            .ToListAsync(cancellationToken);

}
