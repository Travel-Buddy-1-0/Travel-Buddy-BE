using BusinessLogic.Services;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserActivityController : ControllerBase
    {
        private readonly IUserActivityService _userActivityService;

        public UserActivityController(IUserActivityService userActivityService)
        {
            _userActivityService = userActivityService;
        }

        /// <summary>
        /// Get all user activities
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUserActivities()
        {
            var data = await _userActivityService.GetAllUserActivitiesAsync();
            return Ok(data);
        }

        /// <summary>
        /// Get user activity by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserActivityById([FromRoute] int id)
        {
            var data = await _userActivityService.GetUserActivityByIdAsync(id);
            if (data == null)
            {
                return NotFound($"UserActivity with ID {id} not found.");
            }
            return Ok(data);
        }

        /// <summary>
        /// Get user activities by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserActivitiesByUserId([FromRoute] int userId)
        {
            var data = await _userActivityService.GetUserActivitiesByUserIdAsync(userId);
            return Ok(data);
        }

        /// <summary>
        /// Get user activities with filters
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> GetUserActivitiesByFilter([FromBody] UserActivityFilterRequestDto filter)
        {
            var data = await _userActivityService.GetUserActivitiesByFilterAsync(filter);
            return Ok(data);
        }

        /// <summary>
        /// Create new user activity
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUserActivity([FromBody] UserActivityCreateRequestDto request)
        {
            var data = await _userActivityService.CreateUserActivityAsync(request);
            return CreatedAtAction(nameof(GetUserActivityById), new { id = data.ActivityId }, data);
        }

        /// <summary>
        /// Update user activity
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserActivity([FromRoute] int id, [FromBody] UserActivityUpdateRequestDto request)
        {
            try
            {
                var data = await _userActivityService.UpdateUserActivityAsync(id, request);
                return Ok(data);
            }
            catch (BusinessLogic.Exceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Delete user activity
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserActivity([FromRoute] int id)
        {
            try
            {
                await _userActivityService.DeleteUserActivityAsync(id);
                return NoContent();
            }
            catch (BusinessLogic.Exceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
