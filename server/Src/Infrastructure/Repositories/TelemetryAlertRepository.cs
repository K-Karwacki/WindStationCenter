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

    public async Task<bool> DeleteAsync(TelemetryAlert entity)
    {
        try
        {
            dbContext.TelemetryAlerts.Remove(entity);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var existing = await dbContext.TelemetryAlerts.FirstOrDefaultAsync(t => t.Id == id);
            if (existing == null)
                return false;

            dbContext.TelemetryAlerts.Remove(existing);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public async Task<TelemetryAlert> FindByIdAsync(Guid id)
    {
        var telemetryAlert = await dbContext.TelemetryAlerts.FirstOrDefaultAsync(t => t.Id == id);
        return telemetryAlert ?? throw new EntityNotFoundException("Alert not found");
    }

    public async Task<IEnumerable<TelemetryAlert>> GetAllAsync()
    {
        return await dbContext.TelemetryAlerts.ToListAsync();
    }

    public async Task<bool> UpdateAsync(TelemetryAlert entity)
    {
        try
        {
            var existing = await dbContext.TelemetryAlerts.FirstOrDefaultAsync(t => t.Id == entity.Id);
            if (existing == null)
                throw new EntityNotFoundException("Alert not found");

            dbContext.Entry(existing).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException($"Failed to update alert: {e.Message}", e);
        }
    }
}