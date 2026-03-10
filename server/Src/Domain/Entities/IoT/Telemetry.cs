namespace Domain.Entities.IoT;

public sealed record Telemetry
{
    public Guid Id { get; set; }
    public Guid FarmInternalId { get; set; }
    public required string FarmId { get; set; }
    public Guid TurbineInternalId { get; set; }
    public required string TurbineId { get; set; }
    public required string TurbineName { get; set; }
    public DateTime Timestamp { get; set; }
    public double WindSpeed { get; set; }
    public double WindDirection { get; set; }
    public double AmbientTemperature { get; set; }
    public double RotorSpeed { get; set; }
    public double PowerOutput { get; set; }
    public double NacelleDirection { get; set; }
    public double BladePitch { get; set; }
    public double GeneratorTemp { get; set; }
    public double GearboxTemp { get; set; }
    public double Vibration { get; set; }
    public required string Status { get; set; }
    
}
