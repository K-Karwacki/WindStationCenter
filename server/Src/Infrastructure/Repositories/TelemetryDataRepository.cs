using Domain.Entities.IoT;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TelemetryDataRepository(MyDbContext dbContext) : ITelemetryDataRepository
{
    public async Task<Telemetry> AddAsync(Telemetry entity)
    {
        try
        {
            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            var created = await dbContext.Telemetries.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return created.Entity;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public async Task<bool> DeleteAsync(Telemetry entity)
    {
        try
        {
            dbContext.Telemetries.Remove(entity);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var existing = await dbContext.Telemetries.FirstOrDefaultAsync(t => t.Id == id);
            if (existing == null)
                return false;

            dbContext.Telemetries.Remove(existing);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public async Task<Telemetry> FindByIdAsync(Guid id)
    {
        var telemetry = await dbContext.Telemetries.FirstOrDefaultAsync(t => t.Id == id);
        return telemetry ?? throw new EntityNotFoundException("Telemetry not found");
    }

    public async Task<IEnumerable<Telemetry>> GetAllAsync()
    {
        return await dbContext.Telemetries.ToListAsync();
    }

    public async Task<bool> UpdateAsync(Telemetry entity)
    {
        try
        {
            var existing = await dbContext.Telemetries.FirstOrDefaultAsync(t => t.Id == entity.Id);
            if (existing == null)
                throw new EntityNotFoundException("Telemetry not found");

            dbContext.Entry(existing).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException($"Failed to update telemetry: {e.Message}", e);
        }
    }
}