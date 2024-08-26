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

namespace TrackHub.Security.Application.Roles.GetAll;

[Authorize(Resource = Resources.Permissions, Action = Actions.Read)]
public readonly record struct GetRolesQuery() : IRequest<IReadOnlyCollection<RoleVm>>;

// The GetRolesQueryHandler is a class that implements the IRequestHandler interface to handle the GetRolesQuery.
public class GetRolesQueryHandler(IRoleReader reader) : IRequestHandler<GetRolesQuery, IReadOnlyCollection<RoleVm>>
{
    // The Handle method is responsible for handling the GetRolesQuery and returning the result.
    // It asynchronously calls the GetRolesAsync method of the IRoleReader dependency to retrieve all roles.
    public async Task<IReadOnlyCollection<RoleVm>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        => await reader.GetRolesAsync(cancellationToken);

}
