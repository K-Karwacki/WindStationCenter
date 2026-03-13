using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Api.Mqtt;
using Api.Services;
using Application.DTOs.Requests;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/command")]
public class CommandController(ICommandService commandService) : BaseController
{
    
    [HttpGet("{turbineId}/last")]
    public async Task<IActionResult> GetLastCommand(string turbineId)
    {
        try
        {
            var command = await commandService.GetLastCommandAsync(turbineId);
            return Ok(command);
        }
        catch (EntityNotFoundException e) { return NotFound(e.Message); }
    }

    [HttpPost("{turbineId}/send")]
    public async Task<IActionResult> SendCommand(string turbineId, [FromBody] SendCommandRequest request)
    {
        try
        {
            var userId = GetUserId();
            var command = await commandService.SendCommandAsync(turbineId, userId, request);
            return Ok(command);
        }
        catch (UnauthorizedAccessException e) { return Unauthorized(e.Message); }
        catch (ValidationException e)         { return BadRequest(e.Message); }
        catch (EntityNotFoundException e)     { return NotFound(e.Message); }
    }
}