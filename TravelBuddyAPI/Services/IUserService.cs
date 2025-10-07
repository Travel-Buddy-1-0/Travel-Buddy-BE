using BusinessObject.DTOs;
using BusinessObject.Entities;

namespace BusinessLogic.Services;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(int userId);
    Task<UserDto> CreateUserAsync(User newUser);
    Task<UserDto> UpdateUserProfileAsync(string email, UserProfileUpdateDto updatedProfile);
    Task DeleteUserAsync(string email);
    Task<User> LoginAsync(string email, string password);
    Task<User> GetUserByEmailAsync(string email);
}