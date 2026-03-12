using System.ComponentModel.DataAnnotations;
using Api.Mqtt;
using Application.DTOs.Requests;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;

namespace Api.Services;

// CommandService.cs
public class CommandService(
    ICommandRepository commandRepository,
    ITurbineRepository turbineRepository,
    MqttPublisher mqttPublisher
) : ICommandService
{
    
    private static readonly HashSet<string> ValidActions = ["start", "stop", "setPitch", "setInterval"];

    
    
    public async Task<Command> GetLastCommandAsync(string turbineId)
    {
        var turbine = await turbineRepository.GetTurbineByExternalIdAsync(turbineId)
                      ?? throw new EntityNotFoundException($"Turbine '{turbineId}' not found.");

        return await commandRepository.GetLastByTurbineIdAsync(turbine.Id)
               ?? throw new EntityNotFoundException("No commands found for this turbine.");
    }
    
    public async Task<Command> SendCommandAsync(string turbineId, Guid userId, SendCommandRequest request)
    {
        if (!ValidActions.Contains(request.Action))
            throw new ValidationException($"Unknown action '{request.Action}'.");

        var validationError = ValidateActionFields(request);
        if (validationError != null)
            throw new ValidationException(validationError);

        var turbine = await turbineRepository.GetTurbineByExternalIdAsync(turbineId)
                      ?? throw new EntityNotFoundException($"Turbine '{turbineId}' not found.");

        var command = new Command
        {
            Id = Guid.NewGuid(),
            TurbineInternalId = turbine.Id,
            TurbineId = turbineId,
            UserInternalId = userId,
            UserId = userId.ToString(),
            Timestamp = DateTime.UtcNow,
            Action = request.Action,
            Reason = request.Action == "stop" ? (request.Reason ?? "No reason provided") : null,
            PitchAngle = request.Action == "setPitch" ? request.PitchAngle : null,
            IntervalSeconds = request.Action == "setInterval" ? request.IntervalSeconds : null,
        };

        await commandRepository.AddAsync(command);

        MqttPublishCommand mqttCommand = request.Action switch
        {
            "start"       => new StartTurbineCommand(),
            "stop"        => new StopTurbineCommand { Reason = command.Reason! },
            "setPitch"    => new SetBladePitchCommand((double)request.PitchAngle!)
            {
                Angle = 0
            },
            "setInterval" => new SetReportingIntervalCommand(request.IntervalSeconds!.Value)
            {
                Value = 0
            },
            _             => throw new InvalidOperationException("Unreachable")
        };

        await mqttPublisher.PublishCommandAsync(turbine.Farm.ExternalId, turbineId, mqttCommand);

        return command;
    }

    private static string? ValidateActionFields(SendCommandRequest request) => request.Action switch
    {
        "setPitch" when request.PitchAngle is null        => "PitchAngle is required for setPitch.",
        "setPitch" when request.PitchAngle is < 0 or > 30 => "PitchAngle must be between 0 and 30.",
        "setInterval" when request.IntervalSeconds is null => "IntervalSeconds is required for setInterval.",
        "setInterval" when request.IntervalSeconds is < 1 or > 60 => "IntervalSeconds must be between 1 and 60.",
        _ => null
    };
}