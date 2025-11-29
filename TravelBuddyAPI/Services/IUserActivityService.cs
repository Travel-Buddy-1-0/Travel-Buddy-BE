using BusinessObject.DTOs;
using BusinessObject.Entities;

namespace BusinessLogic.Services
{
    public interface IUserActivityService
    {
        Task<List<UserActivityDto>> GetAllUserActivitiesAsync();
        Task<UserActivityDto?> GetUserActivityByIdAsync(int activityId);
        Task<List<UserActivityDto>> GetUserActivitiesByUserIdAsync(int userId);
        Task<List<UserActivityDto>> GetUserActivitiesByFilterAsync(UserActivityFilterRequestDto filter);
        Task<UserActivityDto> CreateUserActivityAsync(UserActivityCreateRequestDto request);
        Task<UserActivityDto> UpdateUserActivityAsync(int activityId, UserActivityUpdateRequestDto request);
        Task DeleteUserActivityAsync(int activityId);
    }
}
