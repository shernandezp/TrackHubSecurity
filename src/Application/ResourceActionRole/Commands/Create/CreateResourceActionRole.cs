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

namespace TrackHub.Security.Application.ResourceActionRole.Commands.Create;

[Authorize(Resource = Resources.Accounts, Action = Actions.Write)] 
public readonly record struct CreateResourceActionRoleCommand(ResourceActionRoleDto ResourceActionRole) : IRequest<ResourceActionRoleVm>;

public class CreateResourceActionRoleCommandHandler(IResourceActionRoleWriter writer, IUserReader userReader, IUser user) : IRequestHandler<CreateResourceActionRoleCommand, ResourceActionRoleVm>
{
    private Guid UserId { get; } = user.Id is null ? throw new UnauthorizedAccessException() : new Guid(user.Id);

    // This method handles the CreateResourceActionRoleCommand by calling the writer's CreateResourceActionRoleAsync method.
    // It returns a ResourceActionRoleVm object.
    // It throws an UnauthorizedAccessException if the user is not an admin.
    public async Task<ResourceActionRoleVm> Handle(CreateResourceActionRoleCommand request, CancellationToken cancellationToken)
    { 
        var isAdmin = await userReader.IsAdminAsync(UserId, cancellationToken);
        return !isAdmin
            ? throw new UnauthorizedAccessException()
            : await writer.CreateResourceActionRoleAsync(request.ResourceActionRole, cancellationToken);
    }

}
