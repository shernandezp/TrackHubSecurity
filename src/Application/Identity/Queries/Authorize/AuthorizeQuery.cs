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

using TrackHubSecurity.Domain.Interfaces;

namespace TrackHubSecurity.Application.Identity.Queries.Authorize;

public readonly record struct AuthorizeQuery(Guid UserId, string PolicyName) : IRequest<bool>
{
}

public class GetUsersQueryHandler(IUserReader reader) : IRequestHandler<AuthorizeQuery, bool>
{
    public async Task<bool> Handle(AuthorizeQuery request, CancellationToken cancellationToken)
        => await reader.AuthorizeAsync(request.UserId, request.PolicyName, cancellationToken);

}
