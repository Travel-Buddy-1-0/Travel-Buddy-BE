using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? UserId { get; set; }

    public int? TourId { get; set; }

    public int? HotelId { get; set; }

    public int? RestaurantId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public string? Image { get; set; }

    public DateTime? ReviewDate { get; set; }

    public virtual Hotel? Hotel { get; set; }

    public virtual Restaurant? Restaurant { get; set; }

    public virtual Tour? Tour { get; set; }

    public virtual User? User { get; set; }
}
