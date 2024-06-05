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

using Ardalis.GuardClauses;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.Configuration;
using TrackHub.Security.Domain.Interfaces;
using TrackHub.Security.Infrastructure.ManagerApi;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddAppManagerContext(this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration.GetValue<string>("AppSettings:GraphQLManagerService");
        Guard.Against.Null(url, message: "Setting 'GraphQLManagerService' not found.");

        services.AddHeaderPropagation(o => o.Headers.Add("Authorization"));

        services.AddHttpClient("manager",
            client => client.Timeout = TimeSpan.FromSeconds(30))
            .AddHeaderPropagation();

        //Header propagation for GraphQL client
        services.AddSingleton<IGraphQLClient>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("manager");
            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(url)
            };
            var jsonSerializer = new SystemTextJsonSerializer();
            return new GraphQLHttpClient(options, jsonSerializer, httpClient);
        });

        services.AddScoped<IManagerWriter, ManagerWriter>();

        return services;
    }
}
