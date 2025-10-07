namespace BusinessObject.DTOs
{
    public class FavoriteFilterRequestDto
    {
        public int? UserId { get; set; }
        public string? TargetType { get; set; } // "HOTEL", "RESTAURANT", "POST"
        public string? TargetId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "CreatedAt"; // "CreatedAt", "FavoriteId"
        public string? SortOrder { get; set; } = "desc"; // "asc", "desc"
    }
}
