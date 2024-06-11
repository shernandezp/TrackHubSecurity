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

public sealed class UserPolicyWriter(IApplicationDbContext context) : IUserPolicyWriter
{
    public async Task<UserPolicyVm> CreateUserPolicyAsync(UserPolicyDto userPolicyDto, CancellationToken cancellationToken)
    {
        var userPolicy = new UserPolicy
        {
            UserId = userPolicyDto.UserId,
            PolicyId = userPolicyDto.PolicyId
        };

        await context.UserPolicies.AddAsync(userPolicy, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new UserPolicyVm(
            userPolicy.UserId,
            userPolicy.PolicyId);
    }

    public async Task DeleteUserPolicyAsync(Guid userId, int policyId, CancellationToken cancellationToken)
    {
        var userPolicy = await context.UserPolicies.FindAsync([userId, policyId], cancellationToken)
            ?? throw new NotFoundException(nameof(UserPolicy), $"{userId},{policyId}");

        context.UserPolicies.Remove(userPolicy);
        await context.SaveChangesAsync(cancellationToken);
    }
}
