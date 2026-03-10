using Application.DTOs.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/station")]
public class StationController(
    ITurbineRepository turbineRepository
    ) : BaseController
{
    [HttpGet(nameof(GetTurbines))]
    public async Task<ActionResult<IEnumerable<TurbineDto>>> GetTurbines()
    {
        try
        {
            var turbines = await turbineRepository.GetAllAsync();
            var turbinesDto = turbines.Select(TurbineDto.Map);
            return Ok(turbinesDto);
        }
        catch (RepositoryException e)
        {
            return NoContent();
        }
    }
}