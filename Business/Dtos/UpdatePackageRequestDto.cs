﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Dtos;

public class UpdatePackageRequestDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? SeatingArrangement { get; set; }
    public string? Placement { get; set; }

    public decimal? Price { get; set; }
    public string? Currency { get; set; }
}
