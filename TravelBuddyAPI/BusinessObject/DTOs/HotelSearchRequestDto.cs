namespace BusinessObject.DTOs;

public class HotelSearchRequestDto
{
    public string? Location { get; set; }
    public DateOnly? CheckIn { get; set; }
    public DateOnly? CheckOut { get; set; }
    public int? Guests { get; set; }
}


