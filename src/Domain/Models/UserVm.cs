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

namespace TrackHub.Security.Domain.Models;

public readonly record struct UserVm(
    Guid UserId,
    string Username,
    string Password,
    string Email,
    string FirstName,
    string? SecondName,
    string LastName,
    string? SecondSurname,
    DateTime? DOB,
    IEnumerable<RoleVm>? Roles,
    IEnumerable<ProfileVm>? Profiles);
