using Domain.Entities.IoT;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TurbineRepository(MyDbContext dbContext) : ITurbineRepository
{
    public async Task<Turbine> AddAsync(Turbine entity)
    {
        try
        {
            // Ensure an Id is present
            if (Guid.Empty == entity.Id)
                entity.Id = Guid.NewGuid();

            var created = await dbContext.Turbines.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return created.Entity;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public async Task<bool> DeleteAsync(Turbine entity)
    {
        try
        {
            dbContext.Turbines.Remove(entity);
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
            var existing = await dbContext.Turbines.FirstOrDefaultAsync(t => t.Id == id);
            if (existing == null)
                return false;

            dbContext.Turbines.Remove(existing);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public async Task<Turbine> FindByIdAsync(Guid id)
    {
        var turbine = await dbContext.Turbines.FirstOrDefaultAsync(t => t.Id == id);
        return turbine ?? throw new EntityNotFoundException("Turbine not found");
    }

    public async Task<IEnumerable<Turbine>> GetAllAsync()
    {
        return await dbContext.Turbines.ToListAsync();
    }

    public async Task<bool> UpdateAsync(Turbine entity)
    {
        try
        {
            var existing = await dbContext.Turbines.FirstOrDefaultAsync(t => t.Id == entity.Id);
            if (existing == null)
                throw new EntityNotFoundException("Turbine not found");

            dbContext.Entry(existing).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException($"Failed to update turbine: {e.Message}", e);
        }
    }

    public async Task<Turbine> GetTurbineByExternalIdAsync(string turbineId)
    {
        var turbine = await dbContext.Turbines.FirstOrDefaultAsync(t => t.TurbineExternalId == turbineId);
        return turbine ?? throw new EntityNotFoundException("Turbine not found");
    }
}