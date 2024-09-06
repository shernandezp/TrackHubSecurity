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

// This class represents a reader for retrieving user information from the database
public sealed class UserReader(IApplicationDbContext context) : IUserReader
{

    // Retrieves the username of a user with the specified ID
    // -Parameters-
    // id: The ID of the user to retrieve the username for
    // cancellationToken: A token to monitor for cancellation requests
    // -Returns-
    // The username of the user with the specified ID
    public async Task<string> GetUserNameAsync(Guid id, CancellationToken cancellationToken)
        => await context.Users
            .Where(u => u.UserId.Equals(id))
            .Select(u => u.Username)
            .FirstAsync(cancellationToken);

    // Retrieves the detailed information of a user with the specified ID, including their roles and policies
    // -Parameters-
    // id: The ID of the user to retrieve
    // cancellationToken: A token to monitor for cancellation requests
    // -Returns-
    // A user view model
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
                u.LoginAttempts,
                u.AccountId,
                u.Active,
                u.Roles.Select(r => new RoleVm(r.RoleId, r.Name)).ToList(),
                u.Policies.Select(p => new PolicyVm(p.PolicyId, p.Name)).ToList()))
            .FirstAsync(cancellationToken);

    // Retrieves a collection of users associated with the specified account ID, including their roles and policies
    // -Parameters-
    // accountId: The ID of the account to retrieve users for
    // cancellationToken: A token to monitor for cancellation requests
    // -Returns-
    // A collection of user view models
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
                u.LoginAttempts,
                u.AccountId,
                u.Active,
                u.Roles.Select(r => new RoleVm(r.RoleId, r.Name)).ToList(),
                u.Policies.Select(p => new PolicyVm(p.PolicyId, p.Name)).ToList()))
            .ToListAsync(cancellationToken);

    // Validates if the specified email address is unique
    public async Task<bool> ValidateEmailAddressAsync(string emailAddress, CancellationToken cancellationToken)
        => !await context.Users
            .Where(u => u.EmailAddress.Equals(emailAddress))
            .AnyAsync(cancellationToken);

    // Validates if the specified username is unique
    // -Parameters-
    // username: The username to validate
    // cancellationToken: A token to monitor for cancellation requests
    // -Returns-
    // A boolean indicating whether the specified username is unique
    public async Task<bool> ValidateUsernameAsync(string username, CancellationToken cancellationToken)
        => !await context.Users
            .Where(u => u.Username.Equals(username))
            .AnyAsync(cancellationToken);

    // Validates if the specified email address is unique for a user other than the one with the specified ID
    // -Parameters-
    // userId: The ID of the user to validate
    // emailAddress: The email address to validate
    // cancellationToken: A token to monitor for cancellation requests
    public async Task<bool> ValidateEmailAddressAsync(Guid userId, string emailAddress, CancellationToken cancellationToken)
        => !await context.Users
            .Where(u => !u.UserId.Equals(userId) && u.EmailAddress.Equals(emailAddress))
            .AnyAsync(cancellationToken);

    // Validates if the specified username is unique for a user other than the one with the specified ID
    // -Parameters-
    // userId: The ID of the user to validate
    // username: The username to validate
    // cancellationToken: A token to monitor for cancellation requests
    public async Task<bool> ValidateUsernameAsync(Guid userId, string username, CancellationToken cancellationToken)
        => !await context.Users
            .Where(u => !u.UserId.Equals(userId) && u.Username.Equals(username))
            .AnyAsync(cancellationToken);

    // Validates whether the specified user is an administrator
    // -Parameters-
    // userId: The ID of the user to validate
    // cancellationToken: A token to monitor for cancellation requests
    // -Returns-
    // A boolean indicating whether the specified user is an administrator
    public async Task<bool> IsAdminAsync(Guid userId, CancellationToken cancellationToken)
        => await context.Users
            .Where(u => u.UserId == userId)
            .SelectMany(u => u.Roles)
            .AnyAsync(r => r.ParentRoleId == null, cancellationToken);

    // Validates whether a user is a manager of the specified user
    // -Parameters-
    // userId: The ID of the user to validate
    // managerId: The ID of the manager to validate
    // cancellationToken: A token to monitor for cancellation requests
    // -Returns-
    // A boolean indicating whether the specified user is a manager
    public async Task<bool> IsManagerAsync(Guid userId, Guid managerId, CancellationToken cancellationToken)
    {
        // Retrieve the account ID of the user
        var user = await context.Users.FindAsync(userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), $"{userId}");

        // Check if the manager is part of the same account and if a role parent of the manager is top level
        return await context.Users
            .Where(u => u.UserId == managerId && u.AccountId == user.AccountId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => context.Roles
                .Where(pr => pr.RoleId == r.ParentRoleId && pr.ParentRoleId == null))
            .AnyAsync(cancellationToken);
    }

}
