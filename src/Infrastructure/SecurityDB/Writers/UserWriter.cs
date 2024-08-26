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

using Common.Domain.Extensions;
using TrackHub.Security.Domain.Records;
using TrackHub.Security.Infrastructure.SecurityDB.Interfaces;

namespace TrackHub.Security.Infrastructure.SecurityDB.Writers;

// This class represents a writer for the User entity in the security database.
public sealed class UserWriter(IApplicationDbContext context) : IUserWriter
{
    // Creates a new user asynchronously.
    // -Parameters:
    //  - userDto: The user data transfer object.
    //  - accountId: The account identifier.
    //  - cancellationToken: The cancellation token.
    // -Returns:
    //  - The created user view model.
    public async Task<UserVm> CreateUserAsync(CreateUserDto userDto, Guid accountId, CancellationToken cancellationToken)
    {
        // Hash the password
        var password = userDto.Password.HashPassword();

        var user = new User(
            userDto.Username,
            password,
            userDto.EmailAddress,
            userDto.FirstName,
            userDto.SecondName,
            userDto.LastName,
            userDto.SecondSurname,
            userDto.DOB,
            0,
            accountId);

        await context.Users.AddAsync(user, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return new UserVm(
            user.UserId,
            user.Username,
            user.EmailAddress,
            user.FirstName,
            user.SecondName,
            user.LastName,
            user.SecondSurname,
            user.DOB,
            user.LoginAttempts,
            user.AccountId,
            [],
            []);
    }

    // Updates an existing user asynchronously.
    // -Parameters:
    //  - userDto: The user data transfer object.
    //  - cancellationToken: The cancellation token.
    public async Task UpdateUserAsync(UpdateUserDto userDto, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([userDto.UserId], cancellationToken)
            ?? throw new NotFoundException(nameof(User), $"{userDto.UserId}");

        context.Users.Attach(user);

        user.Username = userDto.Username;
        user.EmailAddress = userDto.EmailAddress;
        user.FirstName = userDto.FirstName;
        user.SecondName = userDto.SecondName;
        user.LastName = userDto.LastName;
        user.SecondSurname = userDto.SecondSurname;
        user.DOB = userDto.DOB;

        await context.SaveChangesAsync(cancellationToken);
    }

    // Updates the password of an existing user asynchronously.
    // -Parameters:
    //  - userPasswordDto: The user password data transfer object.
    //  - cancellationToken: The cancellation token.
    public async Task UpdatePasswordAsync(UserPasswordDto userPasswordDto, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([userPasswordDto.UserId], cancellationToken)
            ?? throw new NotFoundException(nameof(User), $"{userPasswordDto.UserId}");

        context.Users.Attach(user);

        var password = userPasswordDto.Password.HashPassword();

        user.Password = password;
        user.Active = true;

        await context.SaveChangesAsync(cancellationToken);
    }

    // Deletes a user asynchronously.
    // -Parameters:
    //  - userId: The user identifier.
    //  - cancellationToken: The cancellation token.
    public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([userId], cancellationToken)
            ?? throw new NotFoundException(nameof(User), $"{userId}");

        context.Users.Attach(user);
        context.Users.Remove(user);

        await context.SaveChangesAsync(cancellationToken);
    }
}
