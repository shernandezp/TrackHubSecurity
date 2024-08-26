namespace TrackHub.Security.Domain.Interfaces;

public interface IRoleReader
{
    Task<IReadOnlyCollection<RoleVm>> GetRolesAsync(CancellationToken cancellationToken);
    Task<RoleResourceVm> GetResourcesAsync(int roleId, CancellationToken cancellationToken);
}
