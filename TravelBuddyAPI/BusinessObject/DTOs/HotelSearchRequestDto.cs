namespace BusinessObject.DTOs;

public class HotelSearchRequestDto
{
    public string? Location { get; set; } = null;
    public DateOnly? CheckIn { get; set; }
    public DateOnly? CheckOut { get; set; }
    public int? Guests { get; set; }
    public int? Stars { get; set; } = null;
    public string? Type { get; set; } = null;
    public List<string>? Amenities { get; set; } = null;
}


