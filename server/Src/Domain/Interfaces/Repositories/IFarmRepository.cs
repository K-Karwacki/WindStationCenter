using Domain.Entities.IoT;

namespace Domain.Interfaces.Repositories;

public interface IFarmRepository : IBaseRepository<Farm>
{
    Task<Farm> GetFarmByExternalIdAsync(string externalId);
}