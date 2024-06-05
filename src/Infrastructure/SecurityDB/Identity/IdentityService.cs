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

using Common.Application.Interfaces;
using TrackHub.Security.Domain.Interfaces;

namespace TrackHub.Security.Infrastructure.SecurityDB.Identity;
public class IdentityService(IUserReader userReader) : IIdentityService
{
    public async Task<string> GetUserNameAsync(Guid userId, CancellationToken token)
        => await userReader.GetUserNameAsync(userId, token);

    public async Task<bool> IsInRoleAsync(Guid userId, string resource, string action, CancellationToken token)
    {
        var resourceActionRoles = await userReader.GetResourceActionRolesAsync(resource, action, token);
        var userRoles = await userReader.GetUserRolesAsync(userId, token);
        return resourceActionRoles.Any(role => userRoles.Contains(role));
    }

    public async Task<bool> AuthorizeAsync(Guid userId, string resource, string action, CancellationToken token)
    {
        var resourceActionPolicies = await userReader.GetResourceActionPoliciesAsync(resource, action, token);
        var userPolicies = await userReader.GetUserPoliciesAsync(userId, token);
        return resourceActionPolicies.All(policy => userPolicies.Contains(policy));
    }
}
