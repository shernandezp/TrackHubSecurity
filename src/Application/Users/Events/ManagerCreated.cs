﻿// Copyright (c) 2025 Sergio Hernandez. All rights reserved.
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

namespace TrackHub.Security.Application.Users.Events;
using Common.Domain.Constants;

public sealed class ManagerCreated
{
    // Represents a notification for when a manager is created
    public readonly record struct Notification(Guid UserId) : INotification
    {
        // Handles the ManagerCreated notification
        public class EventHandler(IUserRoleWriter writer, IRoleReader roleReader) : INotificationHandler<Notification>
        {
            // Handles the notification by creating a user role for the manager
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                // Get the manager role
                var role = await roleReader.GetRoleAsync(Roles.Manager, cancellationToken);
                // Create a user role for the manager
                await writer.CreateUserRoleAsync(new UserRoleDto(
                    notification.UserId,
                    role.RoleId
                    ), cancellationToken);
            }
        }
    }
}
