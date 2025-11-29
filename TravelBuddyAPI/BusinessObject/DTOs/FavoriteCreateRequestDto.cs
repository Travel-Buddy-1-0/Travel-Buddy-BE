using System.ComponentModel.DataAnnotations;

namespace BusinessObject.DTOs
{
    public class FavoriteCreateRequestDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Target type cannot exceed 50 characters")]
        public string TargetType { get; set; } = string.Empty; // "HOTEL", "RESTAURANT", "POST"

        [Required]
        public string TargetId { get; set; } = string.Empty;
    }
}
