 using System.Text.Json.Nodes;

namespace BusinessObject.DTOs;

 public class HotelDetailDto
{
    public int HotelId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Address { get; set; }
     public string? Image { get; set; }
     public string? Style { get; set; }
     public decimal? AverageRating { get; set; }
     public List<HotelRoomDto> Rooms { get; set; } = new();
}

 public class HotelRoomDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = null!;
    public string RoomType { get; set; } = null!;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public bool? IsAvailable { get; set; }
    public string? Image { get; set; }
    public DateOnly? LstCheckoutDate { get; set; }

}


