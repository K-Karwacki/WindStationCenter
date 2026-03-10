using Domain.Entities.IoT;

namespace Application.DTOs.Entities;

public sealed record TurbineDto
{
    public string? TurbineExternalId { get; init; }
    public string? Name { get; set; }
    public string? Location { get; set; }

    public static TurbineDto Map(Turbine turbine)
    {
        return new TurbineDto
        {
            TurbineExternalId = turbine.TurbineExternalId,
            Name = turbine.Name,
            Location = turbine.Location
        };
    }
}