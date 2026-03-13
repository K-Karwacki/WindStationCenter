using System.Diagnostics.Metrics;
using System.Text.Json;
using Api.Services;
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
    TelemetryBuffer telemetryBuffer
    ) : MqttController
{
    [MqttRoute("farm/my-awesome-farm/windmill/{turbineId}/telemetry")]
    public async Task ListenForMeasurements(Telemetry telemetryData, string turbineId)
    {
                        var farmExternalId = telemetryData.FarmId.ToString();

                        Farm farm;
                        try
                        {
                            farm = await farmRepository.GetFarmByExternalIdAsync(farmExternalId);
                        }
                        catch (EntityNotFoundException)
                        {
                            farm = await farmRepository.AddAsync(new Farm
                            {
                                ExternalId = farmExternalId,
                                Name = $"Auto-created {farmExternalId}"
                            });
                        }

                        Turbine turbine;
                        try
                        {
                            turbine = await turbineRepository.GetTurbineByExternalIdAsync(turbineId);
                        }
                        catch (EntityNotFoundException)
                        {
                            turbine = await turbineRepository.AddAsync(new Turbine
                            {
                                TurbineExternalId = turbineId,
                                Farm = farm,
                                Name = $"Auto-created {turbineId}",
                                Location = ""
                            });
                        }

                        var telemetry = new Telemetry
                        {
                            FarmId = farmExternalId,
                            FarmInternalId = farm.Id,
                            TurbineInternalId = turbine.Id,
                            TurbineId = telemetryData.TurbineId,
                            TurbineName = telemetryData.TurbineName,
                            Timestamp = telemetryData.Timestamp,
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