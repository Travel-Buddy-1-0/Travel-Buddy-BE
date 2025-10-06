namespace BusinessObject.DTOs
{
    public class FavoriteDto
    {
        public int FavoriteId { get; set; }
        public int UserId { get; set; }
        public string TargetType { get; set; } = string.Empty; // "HOTEL", "RESTAURANT", "POST"
        public string TargetId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        // Thông tin từ Hotel (khi TargetType = "HOTEL")
        public HotelSummaryDto? Hotel { get; set; }
        
        // Thông tin từ Restaurant (khi TargetType = "RESTAURANT")
        public RestaurantSummaryDto? Restaurant { get; set; }
        
        // Thông tin từ Blog/Post (khi TargetType = "POST") - để trống
        public BlogSummaryDto? Post { get; set; }
    }
    
    public class RestaurantSummaryDto
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Description { get; set; }
        public decimal? AveragePrice { get; set; }
        public object? Image { get; set; }
        public object? Style { get; set; }
    }
    
    public class BlogSummaryDto
    {
        public int BlogId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public DateTime? PublishDate { get; set; }
        public object? Image { get; set; }
        public object? Tags { get; set; }
    }
}
