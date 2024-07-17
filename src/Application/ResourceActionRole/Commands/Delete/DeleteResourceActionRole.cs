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


namespace TrackHub.Security.Application.ResourceActionRole.Commands.Delete;

[Authorize(Resource = Resources.Accounts, Action = Actions.Delete)]
public readonly record struct DeleteResourceActionRoleCommand(int ResourceActionRoleId) : IRequest;

public class DeleteResourceActionRoleCommandHandler(IResourceActionRoleWriter writer) : IRequestHandler<DeleteResourceActionRoleCommand>
{
    // This method handles the DeleteResourceActionRoleCommand by deleting the resource action role.
    // It calls the DeleteResourceActionRoleAsync method of the writer.
    public async Task Handle(DeleteResourceActionRoleCommand request, CancellationToken cancellationToken)
        => await writer.DeleteResourceActionRoleAsync(request.ResourceActionRoleId, cancellationToken);
}
