using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController(IEventService eventService) : ControllerBase
{
    private readonly IEventService _eventService = eventService;

    // READ
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var eventsResult = await _eventService.GetAllEventsAsync();
        var events = eventsResult.Data;

        return eventsResult.Succeeded ? Ok(events) : StatusCode(eventsResult.StatusCode, eventsResult.ErrorMessage);
    }


    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Event))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string id)
    {
        var eventResult = await _eventService.GetEventByIdAsync(id);
        var eventModel = eventResult.Data;

        return eventResult.Succeeded ? Ok(eventModel) : StatusCode(eventResult!.StatusCode, eventResult.ErrorMessage);
    }


    // CREATE
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Event))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CreateEventRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _eventService.CreateEventAsync(request);
        return result.Succeeded ? Created((string?)null, result.Data) : StatusCode(result.StatusCode, result.ErrorMessage);
    }


    // UPDATE
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Event))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(UpdateEventRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _eventService.UpdateEventAsync(request);
        return result.Succeeded ? Ok(result.Data) : StatusCode(result.StatusCode, result.ErrorMessage);
    }


    // DELETE
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Event))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _eventService.DeleteEventAsync(id);
        return result.Succeeded ? Ok($"Id {id} successfully deleted.") : StatusCode(result.StatusCode, result.ErrorMessage);
    }
}
