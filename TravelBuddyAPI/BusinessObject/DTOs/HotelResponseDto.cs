using System.Text.Json.Nodes;

namespace BusinessObject.DTOs
{
    public class HotelResponseDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public JsonObject? Image { get; set; }
        public JsonObject? Style { get; set; }
        public List<RoomResponseDto>? Rooms { get; set; }
    }

    public class RoomResponseDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public string RoomType { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public JsonObject? Image { get; set; }
    }
}
