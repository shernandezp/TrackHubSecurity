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

using TrackHub.Security.Infrastructure.SecurityDB.Interfaces;

namespace TrackHub.Security.Infrastructure.SecurityDB.Readers;

public sealed class RoleReader(IApplicationDbContext context) : IRoleReader
{
    // Get all roles
    public async Task<IReadOnlyCollection<RoleVm>> GetRolesAsync(CancellationToken cancellationToken)
        => await context.Roles
            .Select(r => new RoleVm(r.RoleId, r.Name))
            .ToListAsync(cancellationToken);

    // Get all resources for a role
    public async Task<RoleResourceVm> GetResourcesAsync(int roleId, CancellationToken cancellationToken)
        => await context.Roles
            .Where(r => r.RoleId == roleId)
            .Select(r => new RoleResourceVm
            {
                RoleId = r.RoleId,
                Name = r.Name,
                Resources = context.ResourceActionRole
                    .Where(rar => rar.RoleId == r.RoleId)
                    .Select(rar => rar.ResourceAction.Resource)
                    .Distinct()
                    .Select(res => new ResourceVm
                    {
                        ResourceId = res.ResourceId,
                        ResourceName = res.ResourceName,
                        Actions = res.ResourceActions
                            .Select(ra => ra.Action)
                            .Select(a => new ActionVm
                            {
                                ActionId = a.ActionId,
                                ActionName = a.ActionName
                            }).ToList()
                    }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

}
