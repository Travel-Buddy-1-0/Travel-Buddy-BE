using BusinessObject.Data;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email) ?? new User();
        }

        public async Task<User> UpdateUserByEmailAsync(string email, User updatedUser)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            
            if (user != null)
            {
                user.Username = updatedUser.Username;
                user.Email = updatedUser.Email;
                user.FullName = updatedUser.FullName;
                user.PhoneNumber = updatedUser.PhoneNumber;
                user.DateOfBirth = updatedUser.DateOfBirth;
                user.Photo = updatedUser.Photo;
                user.Role = updatedUser.Role;
                user.Sex = updatedUser.Sex;
                
                await _context.SaveChangesAsync();
            }
            
            return user ?? new User();
        }
    }
}