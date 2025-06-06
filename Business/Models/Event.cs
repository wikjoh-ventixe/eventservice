namespace Business.Models;

public class Event
{
    public string Id { get; set; } = null!;
    public string? Image { get; set; }
    public string Title { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = null!;
    public string? Description { get; set; }
    public string Category { get; set; } = null!;
    public bool Active { get; set; } = true;
    public int MaxBookings { get; set; }

    public IEnumerable<Package> Packages { get; set; } = [];
}
