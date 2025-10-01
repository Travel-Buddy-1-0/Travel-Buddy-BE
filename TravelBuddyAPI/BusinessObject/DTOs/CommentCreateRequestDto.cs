namespace BusinessObject.DTOs
{
    public class CommentCreateRequestDto
    {
        public int BlogId { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; } = null!;
        public int? ParentCommentId { get; set; }
    }
}


