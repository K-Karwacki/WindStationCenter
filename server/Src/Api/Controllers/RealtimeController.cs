using Api.Mqtt;
using Application.DTOs.Entities;
using Domain.Entities.IoT;
using Infrastructure.Persistence;
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
    MqttPublisher mqttPublisher
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
        await mqttPublisher.PublishCommandAsync(turbineId, new StopTurbineCommand
        {
            Reason = reason
        });
        return Ok();
    }

    [HttpPost(nameof(SendStartCommandToTheTurbine))]
    public async Task<IActionResult> SendStartCommandToTheTurbine(string turbineId)
    {
        await mqttPublisher.PublishCommandAsync(turbineId, new StartTurbineCommand());
        return Ok();
    }

    [HttpPost(nameof(SendSetIntervalCommandToTheTurbine))]
    public async Task<IActionResult> SendSetIntervalCommandToTheTurbine(string turbineId, int value)
    {
        await mqttPublisher.PublishCommandAsync(turbineId, new SetReportingIntervalCommand
        {
            Value = value
        });
        return Ok();
    }

    public async Task<IActionResult> SendSetBladePitchCommandToTheTurbine(string turbineId, double value)
    {
        await mqttPublisher.PublishCommandAsync(turbineId, new SetBladePitchCommand
        {
            Angle = value
        });
        return Ok();
    }
}