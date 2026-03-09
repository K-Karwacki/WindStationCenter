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
    ITelemetryAlertRepository telemetryAlertRepository
    ) : MqttController
{
    [MqttRoute("farm/my-awesome-farm/windmill/{turbineId}/telemetry")]
    public async Task ListenForMeasurements(Telemetry telemetryData, string turbineId)
    {
       // logger.LogInformation(JsonSerializer.Serialize(telemetryData));
        Console.WriteLine(turbineId);
        
        // Determine farm external id (devices sometimes send FarmId as a Guid in payload)
        var farmExternalId = telemetryData.FarmId;
        
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
    
        // // Create and persist telemetry using internal turbine id
        var telemetry = new Telemetry
        {
            FarmInternalId = farm.Id,
            TurbineInternalId = turbine.Id,
            FarmId = telemetryData.FarmId,
            TurbineId = telemetryData.TurbineId,
            TurbineName = telemetryData.TurbineName,
            TimeStamp = telemetryData.TimeStamp,
            WindSpeed = telemetryData.WindSpeed,
            WindDirection = telemetryData.WindDirection,
            AmbientTemperature = telemetryData.AmbientTemperature,
            RotorSpeed = telemetryData.RotorSpeed,
            PowerOutput = telemetryData.PowerOutput,
            NacelleDirection = telemetryData.NacelleDirection,
            BladePitch = telemetryData.BladePitch,
            GearboxTemp = telemetryData.GearboxTemp,
            GeneratorTemp = telemetryData.GeneratorTemp,
            Vibration = telemetryData.Vibration,
            Status = telemetryData.Status
        };
        
        await telemetryDataRepository.AddAsync(telemetry);
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