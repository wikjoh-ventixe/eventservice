﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class PackageEntity
{
    [Key]
    public int Id { get; set; }

    public string EventId { get; set; } = null!;
    [ForeignKey(nameof(EventId))]
    public EventEntity Event { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string? SeatingArrangement { get; set; }
    public string? Placement { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
}
