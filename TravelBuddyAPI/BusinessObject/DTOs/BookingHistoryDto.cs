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
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Note { get; set; }
}


