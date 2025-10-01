namespace BusinessObject.DTOs;

public class HotelSummaryDto
{
    public int HotelId { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Image { get; set; }
    public string? Style { get; set; }

    public string? Description { get; set; }
    public decimal? AverageRating { get; set; }
    public List<HotelRoomDto> Rooms { get; set; } = new();

}


