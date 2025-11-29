using BusinessObject.Data;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class CommentBlogRepository : ICommentBlogRepository
    {
        private readonly AppDbContext _context;

        public CommentBlogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CommentBlog> CreateAsync(CommentBlog comment)
        {
            _context.CommentBlogs.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<CommentBlog?> GetByIdAsync(int commentId)
        {
            return await _context.CommentBlogs.FirstOrDefaultAsync(c => c.CommentId == commentId);
        }

        public async Task<List<CommentBlog>> GetByBlogIdAsync(string blogId)
        {
            return await _context.CommentBlogs.Include(x=>x.User)
                .Where(c => c.BlogOnlineId == blogId)
                .ToListAsync();
        }

        public async Task UpdateAsync(CommentBlog comment)
        {
            _context.CommentBlogs.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CommentBlog comment)
        {
            _context.CommentBlogs.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}


