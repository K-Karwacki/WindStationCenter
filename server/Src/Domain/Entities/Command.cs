namespace Domain.Entities;

public class Command
{
    public Guid Id { get; set; }
    public Guid TurbineInternalId { get; set; }
    public required string TurbineId { get; set; }
    public Guid UserInternalId { get; set; }
    public required string UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public required string Action { get; set; }
    public int? IntervalSeconds { get; set; }
    public float? PitchAngle { get; set; }
    public string? Reason { get; set; }
    
    public User? User { get; set; }
}
