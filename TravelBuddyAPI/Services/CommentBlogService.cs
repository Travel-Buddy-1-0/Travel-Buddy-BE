using BusinessObject.DTOs;
using BusinessObject.Entities;
using Repositories;

namespace Services
{
    public class CommentBlogService : ICommentBlogService
    {
        private readonly ICommentBlogRepository _repository;

        public CommentBlogService(ICommentBlogRepository repository)
        {
            _repository = repository;
        }

        public async Task<CommentTreeDto> CreateAsync(CommentCreateRequestDto request)
        {
            var entity = new CommentBlog
            {
                BlogOnlineId = request.BlogId,
                UserId = request.UserId,
                Content = request.Content,
                ParentCommentId = request.ParentCommentId,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _repository.CreateAsync(entity);
            return MapToDto(created);
        }

        public async Task<List<CommentTreeDto>> GetByBlogIdAsync(string blogId)
        {
            var allComments = await _repository.GetByBlogIdAsync(blogId);

            var lookup = allComments.ToDictionary(c => c.CommentId, c => MapToDto(c));
            var roots = new List<CommentTreeDto>();

            foreach (var c in allComments)
            {
                if (c.ParentCommentId.HasValue && lookup.ContainsKey(c.ParentCommentId.Value))
                {
                    lookup[c.ParentCommentId.Value].Replies.Add(lookup[c.CommentId]);
                }
                else
                {
                    roots.Add(lookup[c.CommentId]);
                }
            }

            return roots.OrderByDescending(x => x.CreatedAt).ToList();
        }

        public async Task UpdateAsync(int commentId, CommentUpdateRequestDto request)
        {
            var comment = await _repository.GetByIdAsync(commentId);
            if (comment == null) return;
            comment.Content = request.Content;
            comment.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(comment);
        }

        public async Task DeleteAsync(int commentId)
        {
            var comment = await _repository.GetByIdAsync(commentId);
            if (comment == null) return;
            await _repository.DeleteAsync(comment);
        }

        private static CommentTreeDto MapToDto(CommentBlog c)
        {
             return new CommentTreeDto
            {
                CommentId = c.CommentId,
                BlogId = c.BlogOnlineId,
                UserId = c.UserId,
                Content = c.Content,
                UserName = c.User.FullName,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                ParentCommentId = c.ParentCommentId,
                Replies = new List<CommentTreeDto>()
            };
        }
    }
}


