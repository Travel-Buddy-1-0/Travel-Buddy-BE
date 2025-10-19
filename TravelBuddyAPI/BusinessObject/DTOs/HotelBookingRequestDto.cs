using System.Text.Json.Nodes;

namespace BusinessObject.DTOs;

public class HotelBookingRequestDto
{
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public decimal? TotalPrice { get; set; }
    public int Nights { get; set; }
    public int Guests { get; set; }
    public int HotelId { get; set; }
    public int RoomId { get; set; }
    public int? RestaurantId { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Note { get; set; }
    public int? TypePayment { get; set; } //0 là chưa thanh toán 1 laf banking 2 laf vis
    public string? VoucherCode { get; set; }
}


