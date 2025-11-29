using BusinessObject.Data;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class UserActivityRepository : IUserActivityRepository
    {
        private readonly AppDbContext _context;

        public UserActivityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Useractivity>> GetAllUserActivitiesAsync()
        {
            return await _context.Useractivities
                .Include(ua => ua.User)
                .ToListAsync();
        }

        public async Task<Useractivity?> GetUserActivityByIdAsync(int activityId)
        {
            return await _context.Useractivities
                .Include(ua => ua.User)
                .FirstOrDefaultAsync(ua => ua.ActivityId == activityId);
        }

        public async Task<List<Useractivity>> GetUserActivitiesByUserIdAsync(int userId)
        {
            return await _context.Useractivities
                .Include(ua => ua.User)
                .Where(ua => ua.UserId == userId)
                .OrderByDescending(ua => ua.ActivityDate)
                .ToListAsync();
        }

        public async Task<List<Useractivity>> GetUserActivitiesByFilterAsync(int? userId, string? activityType, DateTime? fromDate, DateTime? toDate, int limit, int offset)
        {
            var query = _context.Useractivities
                .Include(ua => ua.User)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(ua => ua.UserId == userId.Value);
            }

            if (!string.IsNullOrEmpty(activityType))
            {
                query = query.Where(ua => ua.ActivityType == activityType);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(ua => ua.ActivityDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(ua => ua.ActivityDate <= toDate.Value);
            }

            return await query
                .OrderByDescending(ua => ua.ActivityDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Useractivity> AddUserActivityAsync(Useractivity userActivity)
        {
            _context.Useractivities.Add(userActivity);
            await _context.SaveChangesAsync();
            return userActivity;
        }

        public async Task<Useractivity> UpdateUserActivityAsync(Useractivity userActivity)
        {
            _context.Useractivities.Update(userActivity);
            await _context.SaveChangesAsync();
            return userActivity;
        }

        public async Task DeleteUserActivityAsync(int activityId)
        {
            var userActivity = await _context.Useractivities.FindAsync(activityId);
            if (userActivity != null)
            {
                _context.Useractivities.Remove(userActivity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
