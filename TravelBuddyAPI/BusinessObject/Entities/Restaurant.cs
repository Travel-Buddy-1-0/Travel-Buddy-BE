using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Restaurant
{
    public int RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? AveragePrice { get; set; }

    public string? Image { get; set; }

    public string? Style { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Bookingdetail> Bookingdetails { get; set; } = new List<Bookingdetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
