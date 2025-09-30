using System.Text.Json.Nodes;

namespace BusinessObject.DTOs;

public class HotelBookingRequestDto
{
    public string CustomerName { get; set; } = null!;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? SpecialRequest { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int Nights { get; set; }
    public int Guests { get; set; }
    public int HotelId { get; set; }
}


