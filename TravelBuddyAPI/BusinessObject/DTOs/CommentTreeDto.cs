using System.Text.Json.Serialization;

namespace BusinessObject.DTOs
{
    public class CommentTreeDto
    {
        public int CommentId { get; set; }
        public int BlogId { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ParentCommentId { get; set; }
        public List<CommentTreeDto> Replies { get; set; } = new();
    }
}


