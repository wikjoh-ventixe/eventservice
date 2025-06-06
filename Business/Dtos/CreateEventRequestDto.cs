using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Dtos;

public class CreateEventRequestDto
{
    public string? Image { get; set; }
    public string Title { get; set; } = null!;

    public DateTime EventDate { get; set; }
    public string Location { get; set; } = null!;
    public string? Description { get; set; }
    public string Category { get; set; } = null!;
    public bool Active { get; set; } = true;
    public int MaxBookings { get; set; }
}
