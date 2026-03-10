using System.Diagnostics.Metrics;
using System.Text.Json;
using Domain.Entities.IoT;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Mqtt.Controllers;

namespace Api.Controllers;

public class IotController(
    ILogger<IotController> logger,
    MyDbContext db,
    IFarmRepository farmRepository,
    ITurbineRepository turbineRepository,
    ITelemetryDataRepository telemetryDataRepository,
    ITelemetryAlertRepository telemetryAlertRepository,
    Api.Services.TelemetryBuffer telemetryBuffer
    ) : MqttController
{
    [MqttRoute("farm/my-awesome-farm/windmill/{turbineId}/telemetry")]
    public async Task ListenForMeasurements(Telemetry telemetryData, string turbineId)
    {
       // logger.LogInformation(JsonSerializer.Serialize(telemetryData));

       
        // Instead of persisting every single incoming message, store latest into buffer
        // The TelemetryBufferFlusher background service will persist the latest entry per turbine every minute.
        telemetryBuffer.Update(turbineId, telemetryData);
    }

    [MqttRoute("farm/my-awesome-farm/windmill/{turbineId}/alert")]
    public async Task ListenForAlerts(TelemetryAlert telemetryAlert, string turbineId)
    {
        logger.LogInformation(JsonSerializer.Serialize(telemetryAlert));
        Console.WriteLine(turbineId);
        
        // Determine farm external id (devices sometimes send FarmId as a Guid in payload)
        var farmExternalId = telemetryAlert.FarmId;
        
        // Ensure farm exists
        Farm farm;
        try
        {
            farm = await farmRepository.GetFarmByExternalIdAsync(farmExternalId);
        }
        catch (EntityNotFoundException e)
        {
            farm = await farmRepository.AddAsync(new Farm
            {
                ExternalId = farmExternalId,
                Name = $"Auto-created {farmExternalId}"
            });
        }

        // Ensure turbine exists and is associated with the farm
        Turbine turbine;
        try
        {
            turbine = await turbineRepository.GetTurbineByExternalIdAsync(turbineId);
        }
        catch (EntityNotFoundException  e)
        {
            turbine = await turbineRepository.AddAsync(new Turbine
            {
                TurbineExternalId = turbineId,
                Farm = farm,
                Name = $"Auto-created {turbineId}",
                Location = ""
            });
        }

        var alert = new TelemetryAlert
        {
            TurbineInternalId = turbine.Id,
            TurbineId = telemetryAlert.TurbineId,
            FarmId = telemetryAlert.FarmId,
            TimeStamp = telemetryAlert.TimeStamp,
            Severity = telemetryAlert.Severity,
            Message = telemetryAlert.Message
        };
        
        await telemetryAlertRepository.AddAsync(alert);

    }
}