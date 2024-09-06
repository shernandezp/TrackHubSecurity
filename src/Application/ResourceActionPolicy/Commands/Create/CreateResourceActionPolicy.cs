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

namespace TrackHub.Security.Application.ResourceActionPolicy.Commands.Create;

[Authorize(Resource = Resources.Accounts, Action = Actions.Write)]
public readonly record struct CreateResourceActionPolicyCommand(ResourceActionPolicyDto ResourceActionPolicy) : IRequest<ResourceActionPolicyVm>;

public class CreateResourceActionPolicyCommandHandler(IResourceActionPolicyWriter writer, IUserReader userReader, IUser user) : IRequestHandler<CreateResourceActionPolicyCommand, ResourceActionPolicyVm>
{
    private Guid UserId { get; } = user.Id is null ? throw new UnauthorizedAccessException() : new Guid(user.Id);

    // This method handles the CreateResourceActionPolicyCommand and returns a ResourceActionPolicyVm
    public async Task<ResourceActionPolicyVm> Handle(CreateResourceActionPolicyCommand request, CancellationToken cancellationToken)
    {
        var isAdmin = await userReader.IsAdminAsync(UserId, cancellationToken);
        return !isAdmin
            ? throw new UnauthorizedAccessException()
            : await writer.CreateResourceActionPolicyAsync(request.ResourceActionPolicy, cancellationToken); 
    }

}
