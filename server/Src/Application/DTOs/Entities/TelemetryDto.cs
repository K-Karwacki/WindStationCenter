namespace Application.DTOs.Entities;

public sealed record TelemetryDto
{
    public string? FarmId { get; init; }
    public string? TurbineId { get; init; }
    public string? TurbineName { get; init; }
    public DateTime? TimeStamp { get; set; }
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
    
}