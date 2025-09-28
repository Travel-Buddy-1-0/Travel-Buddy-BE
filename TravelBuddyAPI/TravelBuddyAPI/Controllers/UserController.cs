using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly Supabase.Client _client;
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] AuthRequestDto request, [FromBody] UserProfileUpdateDto updateDto)
        {
            try
            {
                await _client.Auth.SetSession(request.AccessToken, request.RefreshToken);

                // Cập nhật mật khẩu
                var user = await _client.Auth.GetUser(request.AccessToken);
                if (user == null)
                {
                    return Unauthorized("Invalid access token.");
                }
                var updatedUser = await _userService.UpdateUserProfileAsync(user.Email, updateDto);
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
