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


using Common.Application.Interfaces;

namespace TrackHub.Security.Application.ResourceActionPolicy.Commands.Delete;

[Authorize(Resource = Resources.Permissions, Action = Actions.Delete)]
public readonly record struct DeleteResourceActionPolicyCommand(int ResourceId, int ActionId, int PolicyId) : IRequest;

public class DeleteResourceActionPolicyCommandHandler(IResourceActionPolicyWriter writer, IUserReader userReader, IUser user) : IRequestHandler<DeleteResourceActionPolicyCommand>
{
    private Guid UserId { get; } = user.Id is null ? throw new UnauthorizedAccessException() : new Guid(user.Id);

    // This method handles the DeleteResourceActionPolicyCommand by deleting the resource action policy
    // with the specified ID using the provided writer.
    public async Task Handle(DeleteResourceActionPolicyCommand request, CancellationToken cancellationToken)
    {
        var isAdmin = await userReader.IsAdminAsync(UserId, cancellationToken);
        if (!isAdmin) throw new UnauthorizedAccessException();
        await writer.DeleteResourceActionPolicyAsync(request.ResourceId, request.ActionId, request.PolicyId, cancellationToken);
    }
}
