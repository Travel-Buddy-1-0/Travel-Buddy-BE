using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Room
{
    public int RoomId { get; set; }

    public int? HotelId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public string RoomType { get; set; } = null!;

    public decimal PricePerNight { get; set; }

    public int Capacity { get; set; }

    public bool? IsAvailable { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<Bookingdetail> Bookingdetails { get; set; } = new List<Bookingdetail>();

    public virtual Hotel? Hotel { get; set; }
}
