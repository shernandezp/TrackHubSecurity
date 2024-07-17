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


namespace TrackHub.Security.Application.ResourceActionPolicy.Commands.Delete;

[Authorize(Resource = Resources.Accounts, Action = Actions.Delete)]
public readonly record struct DeleteResourceActionPolicyCommand(int ResourceActionPolicyId) : IRequest;

public class DeleteResourceActionPolicyCommandHandler(IResourceActionPolicyWriter writer) : IRequestHandler<DeleteResourceActionPolicyCommand>
{
    // This method handles the DeleteResourceActionPolicyCommand by deleting the resource action policy
    // with the specified ID using the provided writer.
    public async Task Handle(DeleteResourceActionPolicyCommand request, CancellationToken cancellationToken)
        => await writer.DeleteResourceActionPolicyAsync(request.ResourceActionPolicyId, cancellationToken);
}
