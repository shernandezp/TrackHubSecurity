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

using TrackHub.Security.Application.Users.Commands.Create;
using TrackHub.Security.Application.Users.Commands.Delete;
using TrackHub.Security.Application.Users.Commands.Update;
using TrackHub.Security.Application.Users.Queries.Get;
using TrackHub.Security.Domain.Models;

namespace TrackHub.Security.Web.Endpoints;

public sealed class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetUser)
            .MapPost(CreateUser)
            .MapPut(UpdateUser, "{id}")
            .MapDelete(DeleteUser, "{id}");
    }

    public async Task<UserVm> GetUser(ISender sender, [AsParameters] GetUserQuery query)
        => await sender.Send(query);

    public async Task<UserVm> CreateUser(ISender sender, CreateUserCommand command)
        => await sender.Send(command);

    public async Task<IResult> UpdateUser(ISender sender, Guid id, UpdateUserCommand command)
    {
        if (id != command.User.UserId) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteUser(ISender sender, Guid id)
    {
        await sender.Send(new DeleteUserCommand(id));
        return Results.NoContent();
    }
}
