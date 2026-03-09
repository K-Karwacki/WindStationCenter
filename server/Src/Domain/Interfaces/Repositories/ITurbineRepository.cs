using Domain.Entities.IoT;

namespace Domain.Interfaces.Repositories;

public interface ITurbineRepository : IBaseRepository<Turbine>
{
    Task<Turbine> GetTurbineByExternalIdAsync(string turbineId);
    
}