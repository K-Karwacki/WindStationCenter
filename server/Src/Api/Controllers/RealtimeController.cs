using System.Diagnostics.Metrics;
using Application.DTOs.Entities;
using Domain.Entities.IoT;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using StateleSSE.AspNetCore;
using StateleSSE.AspNetCore.EfRealtime;
using StateleSSE.AspNetCore.GroupRealtime;

namespace Api.Controllers;

[Route("api/realtime")]
public class RealtimeController(ISseBackplane backplane,
    IRealtimeManager realtimeManager,
    MyDbContext db,
    IGroupRealtimeManager groupRealtimeManager
) : RealtimeControllerBase(backplane)
{
    
    [HttpGet(nameof(GetTelemetry))]
    public async Task<RealtimeListenResponse<List<Telemetry>>> GetTelemetry(string connectionId)
    {
        var group = "measurements";
        await backplane.Groups.AddToGroupAsync(connectionId, group);
        realtimeManager.Subscribe<MyDbContext>(connectionId, group, 
            criteria: snapshot => snapshot.HasChanges<Telemetry>(),
            query: async context => context.Telemetries.ToList());
        return new RealtimeListenResponse<List<Telemetry>>(group, db.Telemetries.ToList());
    }
}