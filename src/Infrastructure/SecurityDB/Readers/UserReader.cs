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

using TrackHub.Security.Domain.Interfaces;
using TrackHub.Security.Infrastructure.SecurityDB.Interfaces;

namespace TrackHub.Security.Infrastructure.SecurityDB.Readers;
public sealed class UserReader(IApplicationDbContext context) : IUserReader
{

    public async Task<string> GetUserNameAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Users
            .Where(u => u.UserId.Equals(id))
            .Select(u => u.Username)
            .FirstAsync(cancellationToken);
    }

    public async Task<UserVm> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Users
            .Where(u => u.UserId.Equals(id))
            .Include(role => role.Roles)
            .Include(policy => policy.Policies)
            .Select(u => new UserVm(
                u.UserId,
                u.Username,
                u.EmailAddress,
                u.FirstName,
                u.SecondName,
                u.LastName,
                u.SecondSurname,
                u.DOB,
                u.AccountId,
                u.Roles.Select(r => new RoleVm(r.RoleName)).ToList(),
                u.Policies.Select(p => new ProfileVm(p.PolicyName)).ToList()))
            .FirstAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
        => await context.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId.Equals(userId))
            .Select(ur => ur.Role.RoleName)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<string>> GetUserPoliciesAsync(Guid userId, CancellationToken cancellationToken)
        => await context.UserPolicies
            .Include(up => up.Policy)
            .Where(up => up.UserId.Equals(userId))
            .Select(up => up.Policy.PolicyName)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<string>> GetResourceActionRolesAsync(string resource, string action, CancellationToken cancellationToken)
        => await context.ResourceActionRole
            .Include(rar => rar.Resource)
            .Include(rar => rar.Action)
            .Include(rar => rar.Role)
            .Where(rar => rar.Resource.ResourceName.Equals(resource) && rar.Action.ActionName.Equals(action))
            .Select(rar => rar.Role.RoleName)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<string>> GetResourceActionPoliciesAsync(string resource, string action, CancellationToken cancellationToken)
        => await context.ResourceActionPolicy
            .Include(rap => rap.Policy)
            .Include(rap => rap.Resource)
            .Include(rap => rap.Action)
            .Where(rap => rap.Resource.ResourceName.Equals(resource) && rap.Action.ActionName.Equals(action))
            .Select(rap => rap.Policy.PolicyName)
            .ToListAsync(cancellationToken);
}
