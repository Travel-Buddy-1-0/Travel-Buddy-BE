using System;

namespace BusinessObject.DTOs;

public class BookingHistoryDto
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public int? HotelId { get; set; }
    public DateTime BookingDate { get; set; }
    public DateOnly? CheckInDate { get; set; }
    public DateOnly? CheckOutDate { get; set; }
    public decimal? TotalPrice { get; set; }
    public bool Approved { get; set; }
}


