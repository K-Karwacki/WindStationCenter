using Domain.Entities.IoT;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TelemetryAlertRepository(MyDbContext dbContext) : ITelemetryAlertRepository
{
    public async Task<TelemetryAlert> AddAsync(TelemetryAlert entity)
    {
        try
        {
            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            var created = await dbContext.TelemetryAlerts.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return created.Entity;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public Task<bool> DeleteAsync(TelemetryAlert entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<TelemetryAlert> FindByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TelemetryAlert>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(TelemetryAlert entity)
    {
        throw new NotImplementedException();
    }
}