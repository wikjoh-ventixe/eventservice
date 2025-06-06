namespace Data.Entities;

public class EventPackageEntity
{
    // Composite key set via fluent api
    public string EventId { get; set; } = null!;
    public EventEntity Event { get; set; } = null!;

    public int PackageId { get; set; }
    public PackageEntity Package { get; set; } = null!;
}
