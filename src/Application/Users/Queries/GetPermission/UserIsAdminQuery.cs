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

namespace TrackHub.Security.Application.Users.Queries.GetPermission;

[Authorize(Resource = Resources.Users, Action = Actions.Read)]
public readonly record struct UserIsAdminQuery() : IRequest<bool>;

public class UserIsAdminQueryHandler(IUserReader reader, IUser user) : IRequestHandler<UserIsAdminQuery, bool>
{
    private Guid UserId { get; } = user.Id is null ? throw new UnauthorizedAccessException() : new Guid(user.Id);

    /// <summary>
    /// Handles the UserIsAdminQuery request by checking if the user is an admin.
    /// It uses the IUserReader service to perform the check.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns a boolean value indicating whether the user is an admin or not.</returns>
    public async Task<bool> Handle(UserIsAdminQuery request, CancellationToken cancellationToken)
        => await reader.IsAdminAsync(UserId, cancellationToken);

}
