namespace BusinessObject.DTOs;

public class HotelFilterRequestDto
{
    public int? Star { get; set; }
    public string? Type { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}


