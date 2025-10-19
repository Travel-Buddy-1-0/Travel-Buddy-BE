using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Bookingdetail
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? TourId { get; set; }

    public int? HotelId { get; set; }

    public int? RestaurantId { get; set; }

    public DateTime? BookingDate { get; set; }

    public DateOnly? CheckInDate { get; set; }

    public DateOnly? CheckOutDate { get; set; }

    public decimal? TotalPrice { get; set; }

    public bool? Approved { get; set; }

    public int? RoomId { get; set; }

    public int? Status { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Note { get; set; }

    public decimal OriginalPrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public string? VoucherCode { get; set; }

    public virtual Hotel? Hotel { get; set; }

    public virtual Restaurant? Restaurant { get; set; }

    public virtual Room? Room { get; set; }

    public virtual Tour? Tour { get; set; }

    public virtual User? User { get; set; }
}
