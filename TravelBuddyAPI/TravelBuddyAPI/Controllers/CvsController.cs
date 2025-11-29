using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;

namespace TravelBuddyAPI.Controllers
{
    [Route("api/cvs")]
    [ApiController]
    [Authorize]
    public class CvsController : ControllerBase
    {
        private readonly ICvService _cvService;

        public CvsController(ICvService cvService)
        {
            _cvService = cvService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User is not authenticated or Token is invalid.");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCv([FromBody] CreateCvRequest request)
        {
            try
            {
                var result = await _cvService.CreateManualCvAsync(GetCurrentUserId(), request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{cvId}")]
        public async Task<IActionResult> UpdateCv(int cvId, [FromBody] UpdateCvRequest request)
        {
            try
            {
                var result = await _cvService.UpdateCvAsync(cvId, GetCurrentUserId(), request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); // 409 Conflict
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "CV not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCvs()
        {
            var result = await _cvService.GetMyCvsAsync(GetCurrentUserId());
            return Ok(result);
        }

        [HttpGet("{cvId}")]
        public async Task<IActionResult> GetCvById(int cvId)
        {
            try
            {
                var result = await _cvService.GetCvDetailAsync(cvId, GetCurrentUserId());
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{cvId}")]
        public async Task<IActionResult> DeleteCv(int cvId)
        {
            try
            {
                await _cvService.DeleteCvAsync(cvId, GetCurrentUserId());
                return Ok(new { message = "CV deleted" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCv([FromForm] UploadCvRequest request)
        {
            if (request.File == null || request.File.Length == 0) return BadRequest("File empty");
            try
            {
                var result = await _cvService.UploadCvAsync(GetCurrentUserId(), request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
