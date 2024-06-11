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
public sealed class UserReader(IApplicationDbContext context) : IUserReader
{

    public async Task<string> GetUserNameAsync(Guid id, CancellationToken cancellationToken)
        => await context.Users
            .Where(u => u.UserId.Equals(id))
            .Select(u => u.Username)
            .FirstAsync(cancellationToken);

    public async Task<UserVm> GetUserAsync(Guid id, CancellationToken cancellationToken)
        => await context.Users
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
                u.Policies.Select(p => new PolicyVm(p.PolicyName)).ToList()))
            .FirstAsync(cancellationToken);

    public async Task<IReadOnlyCollection<UserVm>> GetUsersAsync(Guid accountId, CancellationToken cancellationToken)
        => await context.Users
            .Where(u => u.AccountId.Equals(accountId))
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
                u.Policies.Select(p => new PolicyVm(p.PolicyName)).ToList()))
            .ToListAsync(cancellationToken);

    public async Task<bool> ValidateEmailAddressAsync(string emailAddress, CancellationToken cancellationToken)
        => !await context.Users
            .Where(u => u.EmailAddress.Equals(emailAddress))
            .AnyAsync(cancellationToken);

    public async Task<bool> ValidateUsernameAsync(string username, CancellationToken cancellationToken)
        => !await context.Users
            .Where(u => u.Username.Equals(username))
            .AnyAsync(cancellationToken);

    public async Task<bool> ValidateEmailAddressAsync(Guid userId, string emailAddress, CancellationToken cancellationToken)
        => !await context.Users
            .Where(u => !u.UserId.Equals(userId) && u.EmailAddress.Equals(emailAddress))
            .AnyAsync(cancellationToken);

    public async Task<bool> ValidateUsernameAsync(Guid userId, string username, CancellationToken cancellationToken)
        => !await context.Users
            .Where(u => !u.UserId.Equals(userId) && u.Username.Equals(username))
            .AnyAsync(cancellationToken);

}
