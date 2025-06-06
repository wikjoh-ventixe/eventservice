using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Context;

public class EventDbContext(DbContextOptions<EventDbContext> options) : DbContext(options)
{
    public DbSet<EventEntity> Events { get; set; }
    public DbSet<PackageEntity> Packages { get; set; }
    public DbSet<EventPackageEntity> EventPackages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventPackageEntity>()
            .HasKey(ep => new { ep.EventId, ep.PackageId });
    }
}
