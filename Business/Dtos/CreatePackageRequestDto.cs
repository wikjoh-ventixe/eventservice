using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Dtos;

public class CreatePackageRequestDto
{
    public string EventId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? SeatingArrangement { get; set; }
    public string? Placement { get; set; }

    public decimal? Price { get; set; }
    public string? Currency { get; set; }
}
