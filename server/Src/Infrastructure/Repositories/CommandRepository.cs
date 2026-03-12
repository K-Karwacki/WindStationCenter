using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CommandRepository(MyDbContext dbContext) : ICommandRepository
{
    public async Task<Command> AddAsync(Command entity)
    {
        try
        {
            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            var created = await dbContext.Commands.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return created.Entity;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public async Task<bool> DeleteAsync(Command entity)
    {
        try
        {
            dbContext.Commands.Remove(entity);
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
            var existing = await dbContext.Commands.FirstOrDefaultAsync(t => t.Id == id);
            if (existing == null)
                return false;

            dbContext.Commands.Remove(existing);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public async Task<Command> FindByIdAsync(Guid id)
    {
        var command = await dbContext.Commands.FirstOrDefaultAsync(t => t.Id == id);
        return command ?? throw new EntityNotFoundException("Command not found");
    }

    public async Task<IEnumerable<Command>> GetAllAsync()
    {
        return await dbContext.Commands.ToListAsync();
    }

    public async Task<bool> UpdateAsync(Command entity)
    {
        try
        {
            var existing = await dbContext.Commands.FirstOrDefaultAsync(t => t.Id == entity.Id);
            if (existing == null)
                throw new EntityNotFoundException("Command not found");

            dbContext.Entry(existing).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException($"Failed to update command: {e.Message}", e);
        }
    }

    public async Task<Command?> GetLastByTurbineIdAsync(Guid turbineInternalId)
    {
        var command =  await dbContext.Commands
            .Where(c => c.TurbineInternalId == turbineInternalId)
            .OrderByDescending(c => c.Timestamp)
            .FirstOrDefaultAsync();
        return command ?? throw new EntityNotFoundException("Command or Turbine not found");
        
    }
}