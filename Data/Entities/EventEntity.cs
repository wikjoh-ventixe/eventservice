using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class EventEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Image { get; set; }
    public string Title { get; set; } = null!;
    public DateTime EventDate {  get; set; }
    public string Location { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<PackageEntity> Packages { get; set; } = [];
}
