using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface ICommandRepository : IBaseRepository<Command>
{
    Task<Command?> GetLastByTurbineIdAsync(Guid id);
}