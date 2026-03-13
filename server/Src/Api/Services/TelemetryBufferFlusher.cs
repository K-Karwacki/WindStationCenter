
using Domain.Entities.IoT;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api.Services;

public class TelemetryBufferFlusher : BackgroundService
    {
        private readonly TelemetryBuffer _buffer;
        private readonly IServiceProvider _services;
        private readonly ILogger<TelemetryBufferFlusher> _logger;

        public TelemetryBufferFlusher(
            TelemetryBuffer buffer,
            IServiceProvider services,
            ILogger<TelemetryBufferFlusher> logger)
        {
            _buffer = buffer;
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TelemetryBufferFlusher starting; will flush every minute.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (TaskCanceledException) { break; }

                var items = _buffer.DrainAll();
                if (items.Length == 0) continue;

                _logger.LogInformation("Flushing {count} telemetry buffer entries", items.Length);

                using var scope = _services.CreateScope();
                var farmRepo = scope.ServiceProvider.GetRequiredService<IFarmRepository>();
                var turbineRepo = scope.ServiceProvider.GetRequiredService<ITurbineRepository>();
                var telemetryRepo = scope.ServiceProvider.GetRequiredService<ITelemetryDataRepository>();

                foreach (var kv in items)
                {
                    var turbineId = kv.Key;
                    var telemetryData = kv.Value;

                    try
                    {
                        // Determine farm external id
                        var farmExternalId = telemetryData.FarmId.ToString();

                        Farm farm;
                        try
                        {
                            farm = await farmRepo.GetFarmByExternalIdAsync(farmExternalId);
                        }
                        catch (EntityNotFoundException)
                        {
                            farm = await farmRepo.AddAsync(new Farm
                            {
                                ExternalId = farmExternalId,
                                Name = $"Auto-created {farmExternalId}"
                            });
                        }

                        Turbine turbine;
                        try
                        {
                            turbine = await turbineRepo.GetTurbineByExternalIdAsync(turbineId);
                        }
                        catch (EntityNotFoundException)
                        {
                            turbine = await turbineRepo.AddAsync(new Turbine
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

                        await telemetryRepo.AddAsync(telemetry);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to flush telemetry for {turbine}", turbineId);
                    }
                }
            }
        }
    }
    
