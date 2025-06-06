using Business.Dtos;
using Business.Models;

namespace Business.Interfaces;

public interface IEventService
{
    Task<EventResult<Event?>> CreateEventAsync(CreateEventRequestDto request);
    Task<EventResult<bool>> DeleteEventAsync(string id);
    Task<EventResult<IEnumerable<Event>>> GetAllEventsAsync();
    Task<EventResult<Event?>> GetEventByIdAsync(string id);
    Task<EventResult<Event>> UpdateEventAsync(UpdateEventRequestDto request);
}
