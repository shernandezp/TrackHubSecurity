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

using TrackHubSecurity.Application.Users.Queries.GetUser;
using TrackHubSecurity.Application.Users.Queries.GetUsersByAccount;
using TrackHubSecurity.Domain.Models;

namespace TrackHubSecurity.Web.GraphQL.Query;

public class UsersQuery
{
    public async Task<UserVm> GetUser([Service] ISender sender, [AsParameters] GetUserQuery query)
        => await sender.Send(query);

    public async Task<IReadOnlyCollection<UserVm>> GetUsers([Service] ISender sender, [AsParameters] GetUsersByAccountQuery query)
        => await sender.Send(query);

}