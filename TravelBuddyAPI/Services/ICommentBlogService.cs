using BusinessObject.DTOs;

namespace Services
{
    public interface ICommentBlogService
    {
        Task<CommentTreeDto> CreateAsync(CommentCreateRequestDto request);
        Task<List<CommentTreeDto>> GetByBlogIdAsync(int blogId);
        Task UpdateAsync(int commentId, CommentUpdateRequestDto request);
        Task DeleteAsync(int commentId);
    }
}


