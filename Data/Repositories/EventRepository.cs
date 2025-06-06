using Data.Context;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class EventRepository(EventDbContext context) : BaseRepository<EventEntity>(context), IEventRepository
{
}
