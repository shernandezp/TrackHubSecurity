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

using TrackHub.Security.Application.Users.Commands.CreateUser;
using TrackHub.Security.Application.Users.Commands.DeleteUser;
using TrackHub.Security.Application.Users.Commands.UpdateUser;
using TrackHub.Security.Domain.Models;

namespace TrackHub.Security.Web.GraphQL.Mutation;

public class UsersMutation
{
    public async Task<UserVm> CreateUser([Service] ISender sender, CreateUserCommand command)
        => await sender.Send(command);

    public async Task<bool> UpdateUser([Service] ISender sender, Guid id, UpdateUserCommand command)
    {
        if (id != command.User.UserId) return false;
        await sender.Send(command);
        return true;
    }

    public async Task<Guid> DeleteUser([Service] ISender sender, Guid id)
    {
        await sender.Send(new DeleteUserCommand(id));
        return id;
    }
}
