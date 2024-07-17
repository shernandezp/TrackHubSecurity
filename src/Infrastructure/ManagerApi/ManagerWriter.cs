﻿using Common.Infrastructure;
using GraphQL;
using TrackHub.Security.Domain.Models;
using TrackHub.Security.Domain.Interfaces;
using TrackHub.Security.Domain.Records;
using Common.Application.Interfaces;
using Common.Domain.Constants;

namespace TrackHub.Security.Infrastructure.ManagerApi;

// This class represents the ManagerWriter responsible for writing operations related to the manager entity.
public class ManagerWriter(IGraphQLClientFactory graphQLClient)
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IManagerWriter
{
    // Creates a new user asynchronously.
    public async Task<UserShrankVm> CreateUserAsync(UserShrankDto user, CancellationToken token)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    mutation($accountId: UUID!, $active: Boolean!, $userId: UUID!, $username: String!) {
                      createUser(command: { user: { accountId: $accountId, active: $active, userId: $userId, username: $username } }) {
                        userId
                      }
                    }",
            Variables = new
            {
                user.AccountId,
                active = false,
                user.UserId,
                user.Username
            }
        };
        var result = await MutationAsync<UserShrankVm>(request, token);
        return result;
    }

    // Updates an existing user asynchronously.
    public async Task<bool> UpdateUserAsync(Guid id, UpdateUserShrankDto user, CancellationToken token)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    mutation($id:UUID!, $active: Boolean!, $userId: UUID!, $username: String!) {
                      updateUser(id: $id,
                            command: { user: { active: $active, userId: $userId, username: $username } })
                    }",
            Variables = new
            {
                id,
                user.Active,
                user.UserId,
                user.Username
            }
        };
        var result = await MutationAsync<bool>(request, token);
        return result;
    }

    // Deletes a user asynchronously.
    public async Task<Guid> DeleteUserAsync(Guid id, CancellationToken token)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    mutation($id:UUID!) {
                      deleteUser(id: $id)
                    }",
            Variables = new
            {
                id
            }
        };
        var result = await MutationAsync<Guid>(request, token);
        return result;
    }
}
