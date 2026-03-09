namespace Domain.Entities.IoT;

public class Farm
{
    public Guid Id { get; set; }
    public required string ExternalId { get; set; }
    public required string Name { get; set; }
    public ICollection<Turbine> Turbines { get; set; } = [];
}