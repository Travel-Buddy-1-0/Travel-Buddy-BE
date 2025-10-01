using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Location
{
    public int LocationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? Region { get; set; }

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
