using Domain.Entities.IoT;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FarmRepository(MyDbContext dbContext) : IFarmRepository
{
    public async Task<Farm> AddAsync(Farm entity)
    {
        try
        {
            var created = await dbContext.Farms.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return created.Entity;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public async Task<bool> DeleteAsync(Farm entity)
    {
        try
        {
            dbContext.Farms.Remove(entity);
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
            var existing = await dbContext.Farms.FirstOrDefaultAsync(f => f.Id == id);
            if (existing == null)
                return false;

            dbContext.Farms.Remove(existing);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public async Task<Farm> FindByIdAsync(Guid id)
    {
        var farm = await dbContext.Farms
            .Include(f => f.Turbines)
            .FirstOrDefaultAsync(f => f.Id == id);

        return farm ?? throw new EntityNotFoundException("Farm not found");
    }

    public async Task<IEnumerable<Farm>> GetAllAsync()
    {
        return await dbContext.Farms
            .Include(f => f.Turbines)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(Farm entity)
    {
        try
        {
            var existing = await dbContext.Farms.FirstOrDefaultAsync(f => f.Id == entity.Id);
            if (existing == null)
                throw new EntityNotFoundException("Farm not found");

            dbContext.Entry(existing).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException($"Failed to update farm: {e.Message}", e);
        }
    }

    public async Task<Farm> GetFarmByExternalIdAsync(string externalId)
    {
        var farm = await dbContext.Farms
            .Include(f => f.Turbines)
            .FirstOrDefaultAsync(f => f.ExternalId == externalId);

        return farm ?? throw new EntityNotFoundException("Farm not found");
    }
}