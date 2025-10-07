namespace BusinessObject.DTOs;

public class ReviewDto1
{
    public int ReviewId { get; set; }
    public int? UserId { get; set; }
    public string? ReviewerName { get; set; }
    public int? HotelId { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public string? Image { get; set; }
    public DateTime? ReviewDate { get; set; }
}


