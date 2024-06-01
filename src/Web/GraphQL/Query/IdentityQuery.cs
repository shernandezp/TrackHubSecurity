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

using TrackHub.Security.Application.Identity.Queries.Authorize;
using TrackHub.Security.Application.Identity.Queries.GetUsername;
using TrackHub.Security.Application.Identity.Queries.IsInRole;

namespace TrackHub.Security.Web.GraphQL.Query;

public partial class Query
{
    public async Task<string> GetUserName([Service] ISender sender, [AsParameters] GetUserNameQuery query)
        => await sender.Send(query);

    public async Task<bool> IsInRole([Service] ISender sender, [AsParameters] IsInRoleQuey query)
        => await sender.Send(query);

    public async Task<bool> Authorize([Service] ISender sender, [AsParameters] AuthorizeQuery query)
        => await sender.Send(query);

}
