using Application.Bases.Models;
using Application.EndUsers;
using Domain.EndUsers;

namespace Application.Services;

public interface IEndUserService : IService<EndUser>
{
    Task<PagedResult<EndUser>> GetListAsync(Guid workspaceId, Guid envId, EndUserFilter filter);

    Task<EndUser> UpsertAsync(EndUser user);

    Task<ImportUserResult> UpsertAsync(Guid? workspaceId, Guid? envId, IEnumerable<EndUser> endUsers);

    Task<EndUserProperty[]> AddNewPropertiesAsync(EndUser user);

    Task<EndUserProperty[]> AddNewPropertiesAsync(Guid envId, IEnumerable<string> propertyNames);

    Task<ICollection<EndUserProperty>> GetPropertiesAsync(Guid envId);

    Task<EndUserProperty> UpsertPropertyAsync(EndUserProperty property);

    Task DeletePropertyAsync(Guid propertyId);
}