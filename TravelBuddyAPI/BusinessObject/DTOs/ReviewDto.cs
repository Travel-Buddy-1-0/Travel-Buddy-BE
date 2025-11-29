namespace BusinessObject.DTOs;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public int? UserId { get; set; }
    public int? TourId { get; set; }
    public int? HotelId { get; set; }
    public int? RestaurantId { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public string? Image { get; set; }
    public DateTime? ReviewDate { get; set; }
    
    // Related entity names
    public string? UserName { get; set; }
    public string? HotelName { get; set; }
    public string? RestaurantName { get; set; }
    public string? TourName { get; set; }
}

public class ReviewCreateRequestDto
{
    public int? UserId { get; set; }
    public int? TourId { get; set; }
    public int? HotelId { get; set; }
    public int? RestaurantId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? Image { get; set; }
}

public class ReviewUpdateRequestDto
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? Image { get; set; }
}

public class ReviewFilterRequestDto
{
    public int? UserId { get; set; }
    public int? TourId { get; set; }
    public int? HotelId { get; set; }
    public int? RestaurantId { get; set; }
    public int? Rating { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Limit { get; set; } = 20;
    public int Offset { get; set; } = 0;
}


