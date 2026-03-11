using Application.DTOs.Requests;
using Domain.Entities;

namespace Api.Services;

public interface ICommandService
{
    Task<Command> SendCommandAsync(string turbineId, Guid userId, SendCommandRequest request);
    Task<Command> GetLastCommandAsync(string turbineId);
}