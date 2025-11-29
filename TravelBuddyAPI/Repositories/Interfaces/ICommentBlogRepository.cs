using BusinessObject.Entities;

namespace Repositories.Interfaces
{
    public interface ICommentBlogRepository
    {
        Task<CommentBlog> CreateAsync(CommentBlog comment);
        Task<CommentBlog?> GetByIdAsync(int commentId);
        Task<List<CommentBlog>> GetByBlogIdAsync(string blogId);
        Task UpdateAsync(CommentBlog comment);
        Task DeleteAsync(CommentBlog comment);
    }
}


