using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.DTOs;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UserProfileUpdateDto updateDto)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserProfileAsync(id, updateDto);
                return Ok(updatedUser);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Catch other unexpected errors
                return StatusCode(500, "An error occurred while updating the user profile.");
            }
        }

    }
}
