using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("api")] // grouping routes
    public class CommentBlogController : ControllerBase
    {
        private readonly ICommentBlogService _service;

        public CommentBlogController(ICommentBlogService service)
        {
            _service = service;
        }

        // POST /api/blogs/{blogId}/comments
        [HttpPost("blogs/{blogId}/comments")]
        public async Task<ActionResult<CommentTreeDto>> Create(string blogId, [FromBody] CommentCreateRequestDto request)
        {
            if (blogId != request.BlogId)
            {
                return BadRequest(new { message = "BlogId in route and body must match" });
            }
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetByBlogId), new { blogId = created.BlogId }, created);
        }

        // GET /api/blogs/{blogId}/comments (tree)
        [HttpGet("blogs/{blogId}/comments")]
        public async Task<ActionResult<List<CommentTreeDto>>> GetByBlogId(string blogId)
        {
            var result = await _service.GetByBlogIdAsync(blogId);
            return Ok(result);
        }

        // PUT /api/comments/{commentId}
        [HttpPut("comments/{commentId}")]
        public async Task<IActionResult> Update(int commentId, [FromBody] CommentUpdateRequestDto request)
        {
            await _service.UpdateAsync(commentId, request);
            return NoContent();
        }

        // DELETE /api/comments/{commentId}
        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> Delete(int commentId)
        {
            await _service.DeleteAsync(commentId);
            return NoContent();
        }
    }
}


