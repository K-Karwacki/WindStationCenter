namespace Application.DTOs.Requests;

public record SendCommandRequest
{
    public required string Action { get; init; }  // "start" | "stop" | "setPitch" | "setInterval"
    public string? Reason { get; init; }           // stop only
    public double? PitchAngle { get; init; }        // setPitch only
    public int? IntervalSeconds { get; init; }     // setInterval only
}