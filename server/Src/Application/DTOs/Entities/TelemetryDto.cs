using Domain.Entities.IoT;

namespace Application.DTOs.Entities;

public sealed record TelemetryDto
{
    public string? FarmId { get; init; }
    public string? TurbineId { get; init; }
    public string? TurbineName { get; init; }
    public DateTime? Timestamp { get; set; }
    public double? WindSpeed { get; set; }
    public double? WindDirection { get; set; }
    public double? AmbientTemperature { get; set; }
    public double? RotorSpeed { get; set; }
    public double? PowerOutput { get; set; }
    public double? NacelleDirection { get; set; }
    public double? BladePitch { get; set; }
    public double? GeneratorTemp { get; set; }
    public double? Vibration { get; set; }
    public string? Status { get; set; }

    public static TelemetryDto Map(Telemetry telemetry)
    {
        return new TelemetryDto
        {
            FarmId = telemetry.FarmId,
            TurbineId = telemetry.TurbineId,
            TurbineName = telemetry.TurbineName,
            Timestamp = telemetry.Timestamp,
            WindSpeed = telemetry.WindSpeed,
            WindDirection = telemetry.WindDirection,
            AmbientTemperature = telemetry.AmbientTemperature,
            RotorSpeed = telemetry.RotorSpeed,
            PowerOutput = telemetry.PowerOutput,
            NacelleDirection = telemetry.NacelleDirection,
            BladePitch = telemetry.BladePitch,
            GeneratorTemp = telemetry.GeneratorTemp,
            Vibration = telemetry.Vibration,
            Status = telemetry.Status
        };
    }
}