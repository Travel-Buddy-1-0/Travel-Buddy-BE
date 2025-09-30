using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Tour
{
    public int TourId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? Duration { get; set; }

    public decimal? Price { get; set; }

    public int? LocationId { get; set; }

    public virtual ICollection<Bookingdetail> Bookingdetails { get; set; } = new List<Bookingdetail>();

    public virtual Location? Location { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
