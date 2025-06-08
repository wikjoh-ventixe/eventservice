using Business.Dtos;
using Business.Dtos.API;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Protos;
using System.Diagnostics;
using System.Reflection;

namespace Business.Services;

public class EventService(IEventRepository eventRepository, GrpcBooking.GrpcBookingClient grpcBookingClient) : IEventService
{
    private readonly IEventRepository _eventRepository = eventRepository;
    private readonly GrpcBooking.GrpcBookingClient _grpcBookingClient = grpcBookingClient;


    // CREATE
    public async Task<EventResult<Event?>> CreateEventAsync(CreateEventRequestDto request)
    {
        if (request == null)
            return EventResult<Event?>.BadRequest("Request cannot be null.");

        try
        {
            var eventEntity = new EventEntity
            {
                Image = request.Image,
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                EventDate = request.EventDate,
                Category = request.Category,
                Active = request.Active,
                MaxBookings = request.MaxBookings
            };

            await _eventRepository.AddAsync(eventEntity);
            var result = await _eventRepository.SaveAsync();

            if (!result.Succeeded)
                return EventResult<Event?>.InternalServerError($"Failed creating event. {result.ErrorMessage}");

            var createdEventEntity = (await _eventRepository.GetOneAsync(x => x.Id == eventEntity.Id)).Data;
            if (createdEventEntity == null)
                return EventResult<Event?>.InternalServerError($"Failed retrieving event entity after creation.");

            var eventModel = new Event
            {
                Id = createdEventEntity.Id,
                Image = createdEventEntity.Image,
                Title = createdEventEntity.Title,
                Description = createdEventEntity.Description,
                Location = createdEventEntity.Location,
                EventDate = createdEventEntity.EventDate,
                Category = createdEventEntity.Category,
                Active = createdEventEntity.Active,
                MaxBookings = createdEventEntity.MaxBookings
            };

            return EventResult<Event?>.Created(eventModel);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return EventResult<Event?>.InternalServerError($"Exception occurred in {MethodBase.GetCurrentMethod()!.Name}.");
        }
    }


    // READ
    public async Task<EventResult<IEnumerable<Event>>> GetAllEventsAsync()
    {
        var result = await _eventRepository.GetAllAsync(false, null, null, x => x.Packages);
        if (!result.Succeeded)
            return EventResult<IEnumerable<Event>>.InternalServerError($"Failed retrieving event entities. {result.ErrorMessage}");

        var entities = result.Data;
        var events = entities?.Select(x => new Event
        {
            Id = x.Id,
            Image = x.Image,
            Title = x.Title,
            Description = x.Description,
            Location = x.Location,
            EventDate = x.EventDate,
            Category = x.Category,
            Active = x.Active,
            MaxBookings = x.MaxBookings,
            Packages = x.Packages.Select(x => new Package
            {
                Id = x.Id,
                EventId = x.EventId,
                Title = x.Title,
                SeatingArrangement = x.SeatingArrangement,
                Placement = x.Placement,
                Price = x.Price,
                Currency = x.Currency,
            })
        });

        return EventResult<IEnumerable<Event>>.Ok(events!);
    }

    public async Task<EventResult<IEnumerable<Event>>> GetAllEventsWithTicketsSoldAsync()
    {
        var grpcResponse = await _grpcBookingClient.GetTicketsSoldAmountAllEventsAsync(new GetTicketsSoldAmountAllEventsRequest());
        if (!grpcResponse.Succeeded)
            return EventResult<IEnumerable<Event>>.InternalServerError("Failed retrieving tickets sold over gRPC.");

        var result = await _eventRepository.GetAllAsync(false, null, null, x => x.Packages);
        if (!result.Succeeded)
            return EventResult<IEnumerable<Event>>.InternalServerError($"Failed retrieving event entities. {result.ErrorMessage}");

        var entities = result.Data;
        var events = entities?.Select(x => new Event
        {
            Id = x.Id,
            Image = x.Image,
            Title = x.Title,
            Description = x.Description,
            Location = x.Location,
            EventDate = x.EventDate,
            Category = x.Category,
            Active = x.Active,
            MaxBookings = x.MaxBookings,
            TicketsSold = grpcResponse.EventTicketsSold?.FirstOrDefault(z => z.EventId == x.Id)?.TicketsSold ?? 0,
            Packages = x.Packages.Select(x => new Package
            {
                Id = x.Id,
                EventId = x.EventId,
                Title = x.Title,
                SeatingArrangement = x.SeatingArrangement,
                Placement = x.Placement,
                Price = x.Price,
                Currency = x.Currency,
            })
        });

        return EventResult<IEnumerable<Event>>.Ok(events!);
    }

    public async Task<EventResult<Event?>> GetEventByIdAsync(string id)
    {
        var result = await _eventRepository.GetOneAsync(x => x.Id == id, x => x.Packages);
        if (!result.Succeeded || result.Data == null)
            return EventResult<Event?>.NotFound($"Event with id {id} not found.");

        var entity = result.Data;
        var eventModel = new Event
        {
            Id = entity.Id,
            Image = entity.Image,
            Title = entity.Title,
            Description = entity.Description,
            Location = entity.Location,
            EventDate = entity.EventDate,
            Category = entity.Category,
            Active = entity.Active,
            MaxBookings = entity.MaxBookings,
            Packages = entity.Packages.Select(x => new Package
            {
                Id = x.Id,
                EventId = x.EventId,
                Title = x.Title,
                SeatingArrangement = x.SeatingArrangement,
                Placement = x.Placement,
                Price = x.Price,
                Currency = x.Currency,
            })
        };

        return EventResult<Event?>.Ok(eventModel);
    }


    // UPDATE
    public async Task<EventResult<Event>> UpdateEventAsync(UpdateEventRequestDto request)
    {
        if (request == null)
            return EventResult<Event>.BadRequest("Request cannot be null.");

        try
        {
            var existingResult = await _eventRepository.GetOneAsync(x => x.Id == request.Id);
            if (!existingResult.Succeeded || existingResult.Data == null)
                return EventResult<Event>.NotFound($"Event with id {request.Id} not found.");

            var entity = existingResult.Data;
            entity.Image = request.Image;
            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.Location = request.Location;
            entity.EventDate = request.EventDate;
            entity.Category = request.Category;
            entity.Active = request.Active;
            entity.MaxBookings = request.MaxBookings;

            _eventRepository.Update(entity);
            var result = await _eventRepository.SaveAsync();
            if (!result.Succeeded)
                return EventResult<Event>.InternalServerError($"Failed updating event. {result.ErrorMessage}");

            var updatedResult = await _eventRepository.GetOneAsync(x => x.Id == request.Id, x => x.Packages);
            if (!updatedResult.Succeeded || updatedResult.Data == null)
                return EventResult<Event>.InternalServerError("Failed retrieving entity after update.");

            var updatedEntity = updatedResult.Data;
            var updatedEvent = new Event
            {
                Id = updatedEntity.Id,
                Image = updatedEntity.Image,
                Title = updatedEntity.Title,
                Description = updatedEntity.Description,
                Location = updatedEntity.Location,
                EventDate = updatedEntity.EventDate,
                Category = updatedEntity.Category,
                Active = updatedEntity.Active,
                MaxBookings = updatedEntity.MaxBookings,
                Packages = updatedEntity.Packages.Select(x => new Package
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    Title = x.Title,
                    SeatingArrangement = x.SeatingArrangement,
                    Placement = x.Placement,
                    Price = x.Price,
                    Currency = x.Currency,
                })
            };

            return EventResult<Event>.Ok(updatedEvent);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return EventResult<Event>.InternalServerError($"Exception occurred in {MethodBase.GetCurrentMethod()!.Name}.");
        }
    }


    // DELETE
    public async Task<EventResult<bool>> DeleteEventAsync(string id)
    {
        try
        {
            var getResult = await _eventRepository.GetOneAsync(x => x.Id == id);
            if (!getResult.Succeeded || getResult.Data == null)
                return EventResult<bool>.NotFound($"Event with id {id} not found.");

            var entity = getResult.Data;
            _eventRepository.Delete(entity);
            var result = await _eventRepository.SaveAsync();
            if (!result.Succeeded)
                return EventResult<bool>.InternalServerError($"Failed deleting event with id {id}.");

            return EventResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return EventResult<bool>.InternalServerError($"Exception occurred in {MethodBase.GetCurrentMethod()!.Name}.");
        }        
    }
}
