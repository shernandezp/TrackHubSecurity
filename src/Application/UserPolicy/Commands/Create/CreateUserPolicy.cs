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


namespace TrackHub.Security.Application.UserPolicy.Commands.Create;

[Authorize(Resource = Resources.Users, Action = Actions.Write)]
public readonly record struct CreateUserPolicyCommand(UserPolicyDto UserPolicy) : IRequest<UserPolicyVm>;

public class CreateUserPolicyCommandHandler(IUserPolicyWriter writer) : IRequestHandler<CreateUserPolicyCommand, UserPolicyVm>
{
    // This method handles the execution of the CreateUserPolicyCommand.
    public async Task<UserPolicyVm> Handle(CreateUserPolicyCommand request, CancellationToken cancellationToken)
        => await writer.CreateUserPolicyAsync(request.UserPolicy, cancellationToken);

}
