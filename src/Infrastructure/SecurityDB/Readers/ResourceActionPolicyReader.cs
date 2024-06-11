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

using TrackHub.Security.Domain.Interfaces;
using TrackHub.Security.Infrastructure.SecurityDB.Interfaces;

namespace TrackHub.Security.Infrastructure.SecurityDB.Readers;
public sealed class ResourceActionPolicyReader(IApplicationDbContext context) : IResourceActionPolicyReader
{
    public async Task<IReadOnlyCollection<string>> GetResourceActionPoliciesAsync(string resource, string action, CancellationToken cancellationToken)
        => await context.ResourceActionPolicy
            .Include(rap => rap.Policy)
            .Include(rap => rap.Resource)
            .Include(rap => rap.Action)
            .Where(rap => rap.Resource.ResourceName.Equals(resource) && rap.Action.ActionName.Equals(action))
            .Select(rap => rap.Policy.PolicyName)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<ResourceActionVm>> GetPolicyAuthorizedActionsAsync(IReadOnlyCollection<int> policies, CancellationToken cancellationToken)
        => await context.ResourceActionPolicy
            .Include(rap => rap.Resource)
            .Include(rap => rap.Action)
            .Where(rap => policies.Contains(rap.PolicyId))
            .Select(rap => new ResourceActionVm(
                rap.ResourceId,
                rap.Resource.ResourceName,
                rap.ActionId,
                rap.Action.ActionName
            ))
            .ToListAsync(cancellationToken);

}
