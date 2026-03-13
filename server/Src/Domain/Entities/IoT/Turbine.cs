namespace Domain.Entities.IoT;

public sealed record Turbine
{
    public Guid Id { get; set; }
    public Farm? Farm { get; set; }
    public required string TurbineExternalId { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
}