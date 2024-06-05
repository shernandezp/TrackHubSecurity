using Common.Infrastructure;
using GraphQL;
using TrackHub.Security.Domain.Models;
using TrackHub.Security.Domain.Interfaces;
using TrackHub.Security.Domain.Records;
using Common.Application.Interfaces;
using Common.Domain.Constants;

namespace TrackHub.Security.Infrastructure.ManagerApi;

public class ManagerWriter(IGraphQLClientFactory graphQLClient) 
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IManagerWriter
{
    public async Task<ShrankUserVm> CreateUserAsync(ShrankUserDto user, CancellationToken token)
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
        var result = await MutationAsync<ShrankUserVm>(request, token);
        return result;
        //return await MutationAsync<ShrankUserVm>(request, token);
    }

}
