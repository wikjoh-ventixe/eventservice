using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Context;

public class EventDbContext(DbContextOptions<EventDbContext> options) : DbContext(options)
{
    public DbSet<EventEntity> Events { get; set; }
    public DbSet<PackageEntity> Packages { get; set; }
}
