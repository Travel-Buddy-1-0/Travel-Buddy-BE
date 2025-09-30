namespace BusinessObject.DTOs;

public class HotelSearchRequestDto
{
    public string? Location { get; set; }
    public DateOnly? CheckIn { get; set; }
    public DateOnly? CheckOut { get; set; }
    public int? Guests { get; set; }
    public int? Stars { get; set; }
    public string? Type { get; set; }
    public List<string>? Amenities { get; set; }
}


