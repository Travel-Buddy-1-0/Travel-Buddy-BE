using BusinessObject.Entities;

namespace Repositories
{
    public interface ICommentBlogRepository
    {
        Task<CommentBlog> CreateAsync(CommentBlog comment);
        Task<CommentBlog?> GetByIdAsync(int commentId);
        Task<List<CommentBlog>> GetByBlogIdAsync(int blogId);
        Task UpdateAsync(CommentBlog comment);
        Task DeleteAsync(CommentBlog comment);
    }
}


