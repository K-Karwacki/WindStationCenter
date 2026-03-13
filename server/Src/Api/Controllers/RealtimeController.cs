using System.Security.Claims;
using Api.Mqtt;
using Application.DTOs.Entities;
using Domain.Entities;
using Domain.Entities.IoT;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StateleSSE.AspNetCore;
using StateleSSE.AspNetCore.EfRealtime;
using StateleSSE.AspNetCore.GroupRealtime;

namespace Api.Controllers;
// [Authorize]
[Route("api/realtime")]
public class RealtimeController(ISseBackplane backplane,
    IRealtimeManager realtimeManager,
    MyDbContext db,
    IGroupRealtimeManager groupRealtimeManager,
    ITurbineRepository turbineRepository,
    MqttPublisher mqttPublisher,
    ICommandRepository commandRepository
) : RealtimeControllerBase(backplane)
{
    
    [HttpGet(nameof(GetTelemetryDataRealtime))]
    public async Task<RealtimeListenResponse<List<TelemetryDto>>> GetTelemetryDataRealtime(string connectionId)
    {
        const string group = "measurements";
        await backplane.Groups.AddToGroupAsync(connectionId, group);
        List<TelemetryDto> latest = null;
        realtimeManager.Subscribe<MyDbContext>(connectionId, group,
            criteria: snapshot => snapshot.HasChanges<Telemetry>(),
            query: async context =>
            {
                latest = await context.Telemetries
                    .OrderByDescending(t => t.Timestamp)
                    .Take(300)
                    .Select(t => TelemetryDto.Map(t))
                    .ToListAsync();
                return latest;
            });

        return new RealtimeListenResponse<List<TelemetryDto>>(group, latest);
    }

    [HttpGet(nameof(GetTelemetryAlertsRealtime))]
    public async Task<RealtimeListenResponse<List<TelemetryAlert>>> GetTelemetryAlertsRealtime(string connectionId)
    {
        const string group = "alerts";
        await backplane.Groups.AddToGroupAsync(connectionId, group);
        realtimeManager.Subscribe<MyDbContext>(connectionId, group, 
            criteria: snapshot => snapshot.HasChanges<TelemetryAlert>(),
            query: async context => context.TelemetryAlerts.ToList());
        return new RealtimeListenResponse<List<TelemetryAlert>>(group, null);
    }

    [HttpPost(nameof(SendStopCommandToTheTurbine))]
    public async Task<IActionResult> SendStopCommandToTheTurbine(string turbineId, string reason)
    {
        try
        {
            var command = new StopTurbineCommand
            {
                Reason = reason
            };
            var turbine = await turbineRepository.GetTurbineByExternalIdAsync(turbineId);
            await commandRepository.AddAsync(new Command
            {
                Action = command.Action,
                TurbineId = turbineId,
                TurbineInternalId = turbine.Id,
                Reason = reason,
            });
            await mqttPublisher.PublishCommandAsync(turbineId, command);
            return Ok();
        }
        catch (RepositoryException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost(nameof(SendStartCommandToTheTurbine))]
    public async Task<IActionResult> SendStartCommandToTheTurbine(string turbineId)
    {
        try
        {
            var command = new StartTurbineCommand();
            var turbine = await turbineRepository.GetTurbineByExternalIdAsync(turbineId);

            await commandRepository.AddAsync(new Command
            {
                Action = command.Action,
                TurbineId = turbineId,
                TurbineInternalId = turbine.Id,
            });
            await mqttPublisher.PublishCommandAsync(turbineId, command);
            return Ok();
        }
        catch (RepositoryException e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    [HttpPost(nameof(SendSetIntervalCommandToTheTurbine))]
    public async Task<IActionResult> SendSetIntervalCommandToTheTurbine(string turbineId, int value)
    {
        try
        {
            var command = new SetReportingIntervalCommand
            {
                Value =  value
            };
            var turbine = await turbineRepository.GetTurbineByExternalIdAsync(turbineId);

            await commandRepository.AddAsync(new Command
            {
                Action = command.Action,
                IntervalSeconds =  value,
                TurbineId = turbineId,
                TurbineInternalId = turbine.Id,
            });
            await mqttPublisher.PublishCommandAsync(turbineId, command);
            return Ok();
        }
        catch (RepositoryException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IActionResult> SendSetBladePitchCommandToTheTurbine(string turbineId, double value)
    {
        try
        {
            var command = new SetBladePitchCommand
            {
                Angle = value
            };
            var turbine = await turbineRepository.GetTurbineByExternalIdAsync(turbineId);

            await commandRepository.AddAsync(new Command
            {
                Action = command.Action,
                PitchAngle = value,
                TurbineInternalId = turbine.Id,
                TurbineId = turbineId,
            });
            await mqttPublisher.PublishCommandAsync(turbineId, command);
            return Ok();
        }
        catch (RepositoryException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}