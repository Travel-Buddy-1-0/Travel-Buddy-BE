using BusinessObject.Entities;

namespace Repositories
{
    public interface IUserActivityRepository
    {
        Task<List<Useractivity>> GetAllUserActivitiesAsync();
        Task<Useractivity?> GetUserActivityByIdAsync(int activityId);
        Task<List<Useractivity>> GetUserActivitiesByUserIdAsync(int userId);
        Task<List<Useractivity>> GetUserActivitiesByFilterAsync(int? userId, string? activityType, DateTime? fromDate, DateTime? toDate, int limit, int offset);
        Task<Useractivity> AddUserActivityAsync(Useractivity userActivity);
        Task<Useractivity> UpdateUserActivityAsync(Useractivity userActivity);
        Task DeleteUserActivityAsync(int activityId);
    }
}
