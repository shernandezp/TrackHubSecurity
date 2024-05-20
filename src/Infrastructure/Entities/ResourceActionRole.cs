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

namespace TrackHubSecurity.Infrastructure.Entities;

public sealed class ResourceActionRole
{
    private Resource? _resource;
    private Action? _action;
    private Role? _role;

    public int ResourceActionRoleId { get; set; }
    public int ResourceId { get; set; }
    public int ActionId { get; set; }
    public int RoleId { get; set; }

    // Navigation properties
    public Resource Resource
    {
        get => _resource ?? throw new InvalidOperationException("Resource is not loaded");
        set => _resource = value;
    }
    public Action Action
    {
        get => _action ?? throw new InvalidOperationException("Action is not loaded");
        set => _action = value;
    }
    public Role Role
    {
        get => _role ?? throw new InvalidOperationException("Role is not loaded");
        set => _role = value;
    }
}
