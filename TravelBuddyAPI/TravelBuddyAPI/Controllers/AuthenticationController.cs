using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using System.Net;
using TravelBuddyAPI.DTOs;
using Supabase;
namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly Supabase.Client _client;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(Supabase.Client client, ILogger<AuthenticationController> logger)
        {
            _client = client;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<IResult> Register(UserDto request)
        {
            try
            {
                var options = new SignUpOptions
                {
                    RedirectTo = "https://localhost:7056/Authentication/confirmRegister"
                };

                var session = await _client.Auth.SignUp(request.Email, request.Password, options);
                return Results.Ok(session);
            }
            catch (Supabase.Gotrue.Exceptions.GotrueException ex)
            {
                Console.WriteLine($"GoTrue Error: {ex.Message}");
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to register");

                // Return a Bad Request result with a specific problem detail
                return Results.BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Registration Failed",
                    Detail = "An error occurred while registering the user.",
                    Instance = Request.Path
                });
            }
        }
        [HttpPost("confirmRegister")]
        public async Task<IActionResult> ConfirmRegister([FromQuery] ConfirmRegisterRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
                    return BadRequest(new { error = "Missing access or refresh token" });

                // Set lại session (auth state) từ token
                await _client.Auth.SetSession(request.AccessToken, request.RefreshToken);

                // Lấy thông tin user từ access_token
                var user1 = await _client.Auth.GetUser(request.AccessToken);
                var newUser = new BusinessObject.Models.User
                {
                    Email = user1.Email,
                    RegistrationDate = DateTime.Now
                };
                if (user1 == null)
                    return BadRequest(new { error = "User not found" });
                var response = await _client.From<BusinessObject.Models.User>().Insert(newUser);

                _logger.LogInformation($"User {user1.Email} registered and saved to DB.");
                return Ok(response.Models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ConfirmRegister");


                // Return a Bad Request result with a specific problem detail
                return BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Redirection Failed",
                    Detail = "An     error occurred while redirecting the user.",
                    Instance = Request.Path
                });
            }
        }

        [HttpPost("login")]
        public async Task<IResult> Login(RegisterRequestDto request)
        {
            try
            {
                var session = await _client.Auth.SignIn(request.Email, request.Password);
                if (session == null)
                {
                    return Results.BadRequest("Unable to login");
                }
                var userResponse = new UserResponseDto
                {
                    AccessToken = session.AccessToken,
                    RefreshToken = session.RefreshToken,
                    ExpiresIn = session.ExpiresIn,
                    TokenType = session.TokenType
                };
                return Results.Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to login" +
                    $"");

                // Return a Bad Request result with a specific problem detail
                return Results.BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Login Failed",
                    Detail = "An error occurred while logging in the user.",
                    Instance = Request.Path
                });
            }
        }
        [HttpPost("resetpassword")]
        public async Task<IResult> ResetPassword(string email) 
        {
            try
            {
                // Gửi email reset password (Supabase sẽ dùng Site URL đã cấu hình)
                var success = await _client.Auth.ResetPasswordForEmail(email);

                if (!success)
                    return Results.BadRequest("Failed to send reset password email");

                return Results.Ok("Password reset email sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to send reset password email");

                return Results.BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Reset Password Failed",
                    Detail = "An error occurred while sending the reset password email.",
                    Instance = Request.Path
                });
            }
        }

        [HttpPost("updatepassword")]
        public async Task<IResult> UpdatePassword([FromBody] UpdatePasswordDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.NewPassword))
                    return Results.BadRequest("Missing access token or new password");

                // Set session từ access token để xác thực user
                await _client.Auth.SetSession(request.AccessToken, request.RefreshToken);

                // Cập nhật mật khẩu
                var user = await _client.Auth.Update(new UserAttributes
                {
                    Password = request.NewPassword
                });

                return Results.Ok("Password updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to update password");

                return Results.BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Update Password Failed",
                    Detail = "An error occurred while updating the password.",
                    Instance = Request.Path
                });
            }
        }
        private string HashPassword(string password)
        {
            // Ví dụ đơn giản dùng SHA256 (hoặc dùng Identity/KeyDerivation)
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        [HttpPost("logout")]
        public async Task<IResult> Logout(RegisterRequestDto request)
        {
            try
            {
                await _client.Auth.SignOut();
                return Results.Ok("Logged out");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to logout" +
                    $"");

                // Return a Bad Request result with a specific problem detail
                return Results.BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Login Failed",
                    Detail = "An error occurred while logging in the user.",
                    Instance = Request.Path
                });
            }
        }
    }
}
