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


using TrackHub.Security.Domain.Records;
using TrackHub.Security.Infrastructure.SecurityDB.Interfaces;
using TrackHub.Security.Domain.Interfaces;

namespace TrackHub.Security.Infrastructure.SecurityDB.Writers;

public sealed class UserRoleWriter(IApplicationDbContext context) : IUserRoleWriter
{
    public async Task<UserRoleVm> CreateUserRoleAsync(UserRoleDto userRoleDto, CancellationToken cancellationToken)
    {
        var userRole = new UserRole
        {
            UserId = userRoleDto.UserId,
            RoleId = userRoleDto.RoleId
        };

        await context.UserRoles.AddAsync(userRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new UserRoleVm(
            userRole.UserId,
            userRole.RoleId);
    }

    public async Task DeleteUserRoleAsync(Guid userId, int roleId, CancellationToken cancellationToken)
    {
        var userRole = await context.UserRoles.FindAsync([userId, roleId], cancellationToken)
            ?? throw new NotFoundException(nameof(UserRole), $"{userId},{roleId}");

        context.UserRoles.Remove(userRole);
        await context.SaveChangesAsync(cancellationToken);
    }
}