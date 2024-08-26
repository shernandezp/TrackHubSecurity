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

using Common.Application.Interfaces;
using TrackHub.Security.Application.Users.Events;

namespace TrackHub.Security.Application.Users.Commands.Create;

[Authorize(Resource = Resources.Users, Action = Actions.Write)]
public readonly record struct CreateUserCommand(CreateUserDto User) : IRequest<UserVm>;

public class CreateUserCommandHandler(IUserWriter writer, IUserReader reader, IUser user, IPublisher publisher) : IRequestHandler<CreateUserCommand, UserVm>
{
    private Guid UserId { get; } = user.Id is null ? throw new UnauthorizedAccessException() : new Guid(user.Id);

    // Handle the Create User command
    public async Task<UserVm> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // User can be created only within the same account
        var currentUser = await reader.GetUserAsync(UserId, cancellationToken);
        // Create the user using the writer
        var user = await writer.CreateUserAsync(request.User, currentUser.AccountId, cancellationToken);
        // Create a shrank user DTO
        var shrankUser = new UserShrankDto(user.UserId, user.Username, currentUser.AccountId);
        // Publish a UserCreated notification
        await publisher.Publish(new UserCreated.Notification(shrankUser), cancellationToken);
        // Return the created user
        return user;
    }
}
