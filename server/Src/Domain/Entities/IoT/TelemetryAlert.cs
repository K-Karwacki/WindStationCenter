namespace Domain.Entities.IoT;

public class TelemetryAlert
{
    public Guid Id { get; set; }
    public Guid TurbineInternalId { get; set; }
    public required string TurbineId { get; set; }
    public required string FarmId { get; set; }
    public DateTime TimeStamp { get; set; }
    public required string Severity { get; set; }
    public required string Message { get; set; }
}