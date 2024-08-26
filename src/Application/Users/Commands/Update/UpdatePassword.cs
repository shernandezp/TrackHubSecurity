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


using Common.Application.Interfaces;

namespace TrackHub.Security.Application.Users.Commands.Update;
[Authorize(Resource = Resources.Users, Action = Actions.Edit)]
public readonly record struct UpdatePasswordCommand(UserPasswordDto User) : IRequest;
public class UpdatePasswordCommandHandler(IUserWriter writer, IUserReader reader, IUser user) : IRequestHandler<UpdatePasswordCommand>
{
    private Guid UserId { get; } = user.Id is null ? throw new UnauthorizedAccessException() : new Guid(user.Id);

    // Handle the UpdateUserCommand
    // Update the user password if the user is the same user or the requesting user belongs to a parent role whiting the same account
    public async Task Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        // Check if the requesting user belongs to a parent role whiting the same account
        var isParent = await reader.IsParentAsync(request.User.UserId, UserId, cancellationToken);
        if (request.User.UserId == UserId || isParent)
        {
            // Update the user asynchronously
            await writer.UpdatePasswordAsync(request.User, cancellationToken);
        }
        else 
        {
            throw new UnauthorizedAccessException();
        }
    }
}
