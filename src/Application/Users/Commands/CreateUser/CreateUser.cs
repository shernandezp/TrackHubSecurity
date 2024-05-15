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

using TrackHubSecurity.Application.Users.Events;
using TrackHubSecurity.Domain.Interfaces;
using TrackHubSecurity.Domain.Models;
using TrackHubSecurity.Domain.Records;

namespace TrackHubSecurity.Application.Users.Commands.CreateUser;

public readonly record struct CreateUserCommand : IRequest<UserVm>
{
    public required UserDto User { get; init; }
}

public class CreateUserCommandHandler(IUserWriter writer, IPublisher publisher) : IRequestHandler<CreateUserCommand, UserVm>
{
    public async Task<UserVm> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await writer.CreateUserAsync(request.User, cancellationToken);
        
        await publisher.Publish(new UserCreated.Notification(user.UserId), cancellationToken);

        return user;
    }
}