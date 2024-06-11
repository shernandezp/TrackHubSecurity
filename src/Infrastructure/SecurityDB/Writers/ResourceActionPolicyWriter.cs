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


using TrackHub.Security.Domain.Records;
using TrackHub.Security.Infrastructure.SecurityDB.Interfaces;
using TrackHub.Security.Domain.Interfaces;

namespace TrackHub.Security.Infrastructure.SecurityDB.Writers;

public sealed class ResourceActionPolicyWriter(IApplicationDbContext context) : IResourceActionPolicyWriter
{
    public async Task<ResourceActionPolicyVm> CreateResourceActionPolicyAsync(ResourceActionPolicyDto resourceActionPolicyDto, CancellationToken cancellationToken)
    {
        var resourceActionPolicy = new ResourceActionPolicy
        {
            ResourceId = resourceActionPolicyDto.ResourceId,
            ActionId = resourceActionPolicyDto.ActionId,
            PolicyId = resourceActionPolicyDto.PolicyId
        };

        await context.ResourceActionPolicy.AddAsync(resourceActionPolicy, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new ResourceActionPolicyVm(
            resourceActionPolicy.ResourceActionPolicyId,
            resourceActionPolicy.ResourceId,
            resourceActionPolicy.ActionId,
            resourceActionPolicy.PolicyId);
    }

    public async Task DeleteResourceActionPolicyAsync(int resourceActionPolicyId, CancellationToken cancellationToken)
    {
        var resourceActionPolicy = await context.ResourceActionPolicy.FindAsync(resourceActionPolicyId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResourceActionPolicy), $"{resourceActionPolicyId}");

        context.ResourceActionPolicy.Remove(resourceActionPolicy);
        await context.SaveChangesAsync(cancellationToken);
    }
}
