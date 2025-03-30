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

using TrackHub.Security.Infrastructure.SecurityDB.Interfaces;

namespace TrackHub.Security.Infrastructure.SecurityDB.Readers;

/// <summary>
/// This class represents a reader for retrieving client information from the database
/// </summary>
/// <param name="context"></param>
public sealed class ClientReader(IApplicationDbContext context) : IClientReader
{

    /// <summary>
    /// Validates if the specified client exists in the database
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A boolean indicating whether the client exists or not</returns>
    public async Task<bool> IsValidClientAsync(string client, CancellationToken cancellationToken)
        => await context.Clients
            .Where(c => c.Name.Equals(client))
            .AnyAsync(cancellationToken);

}
