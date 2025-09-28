using BusinessLogic.Exceptions;
using BusinessObject.DTOs;
using BusinessObject.Models;
using Repositories;
using System.Security.Authentication;
using System.Text.Json.Nodes;

namespace BusinessLogic.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return users;
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }
        return new UserDto
        {
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            Sex = user.Sex,
            Photo = user.Photo
        };
    }

    public async Task<UserDto> CreateUserAsync(User newUser)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(newUser.Email);
        if (existingUser != null)
        {
            throw new ConflictException("Email already exists.");
        }
        var createdUser = await _userRepository.AddUserAsync(newUser);

        return new UserDto
        {
            Email = createdUser.Email,
            FullName = createdUser.FullName,
            PhoneNumber = createdUser.PhoneNumber,
            DateOfBirth = createdUser.DateOfBirth,
            Sex = createdUser.Sex,
            Photo = createdUser.Photo
        };
    }

    public async Task<User> UpdateUserAsync(string email, User updatedUser)
    {
        var existingUser = await GetUserByEmailAsync(email);

        existingUser.Username = updatedUser.Username;
        existingUser.Email = updatedUser.Email;
        existingUser.FullName = updatedUser.FullName;
        existingUser.PhoneNumber = updatedUser.PhoneNumber;
        existingUser.DateOfBirth = updatedUser.DateOfBirth;
        existingUser.Photo = updatedUser.Photo;
        existingUser.Role = updatedUser.Role;
        existingUser.Sex = updatedUser.Sex;
        return await _userRepository.UpdateUserAsync(existingUser);
    }

    public async Task DeleteUserAsync(string email)
    {
        var user = await GetUserByEmailAsync(email);
        await _userRepository.DeleteUserAsync(user.UserId);
    }

    public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            throw new AuthenticationException("Invalid email or password.");
        }
        return user;
    }
    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            throw new NotFoundException($"User with email {email} not found.");
        }
        return user;
    }

    public async Task<UserDto> UpdateUserProfileAsync(string email,UserProfileUpdateDto updatedProfile)
    {
        
        var userToUpdate = await _userRepository.GetUserByEmailAsync(email);
        if (userToUpdate == null)
        {
            throw new NotFoundException($"User with Email {updatedProfile.Email} not found.");
        }


        userToUpdate.Username = updatedProfile.Username ?? userToUpdate.Username;
        userToUpdate.Email = updatedProfile.Email ?? userToUpdate.Email;
        userToUpdate.FullName = updatedProfile.FullName ?? userToUpdate.FullName;
        userToUpdate.PhoneNumber = updatedProfile.PhoneNumber ?? userToUpdate.PhoneNumber;
        userToUpdate.DateOfBirth = updatedProfile.DateOfBirth ?? userToUpdate.DateOfBirth;
        userToUpdate.Sex = updatedProfile.Sex ?? userToUpdate.Sex;
        userToUpdate.Photo = updatedProfile.Image ?? userToUpdate.Photo;

        // Use a dedicated method in the repository for updates
        var updatedUser = await _userRepository.UpdateUserByEmailAsync(updatedProfile.Email, userToUpdate);
        return new UserDto
        {
            Email = updatedUser.Email,
            FullName = updatedUser.FullName,
            PhoneNumber = updatedUser.PhoneNumber,
            DateOfBirth = updatedUser.DateOfBirth,
            Sex = updatedUser.Sex,
            Photo = updatedUser.Photo
        };
    }
}