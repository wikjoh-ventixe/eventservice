using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Dtos;

public class UpdateEventRequestDto
{
    public string Id { get; set; } = null!;
    public string? Image { get; set; }
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime2")]
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = null!;
    public string? Description { get; set; }
    public string Category { get; set; } = null!;
    public bool Active { get; set; } = true;
    public int MaxBookings { get; set; }
}
