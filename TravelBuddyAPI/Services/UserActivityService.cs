using BusinessLogic.Exceptions;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using Repositories;
using System.Text.Json;

namespace BusinessLogic.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IUserActivityRepository _userActivityRepository;

        public UserActivityService(IUserActivityRepository userActivityRepository)
        {
            _userActivityRepository = userActivityRepository;
        }

        public async Task<List<UserActivityDto>> GetAllUserActivitiesAsync()
        {
            var userActivities = await _userActivityRepository.GetAllUserActivitiesAsync();
            return userActivities.Select(MapToDto).ToList();
        }

        public async Task<UserActivityDto?> GetUserActivityByIdAsync(int activityId)
        {
            var userActivity = await _userActivityRepository.GetUserActivityByIdAsync(activityId);
            return userActivity != null ? MapToDto(userActivity) : null;
        }

        public async Task<List<UserActivityDto>> GetUserActivitiesByUserIdAsync(int userId)
        {
            var userActivities = await _userActivityRepository.GetUserActivitiesByUserIdAsync(userId);
            return userActivities.Select(MapToDto).ToList();
        }

        public async Task<List<UserActivityDto>> GetUserActivitiesByFilterAsync(UserActivityFilterRequestDto filter)
        {
            var userActivities = await _userActivityRepository.GetUserActivitiesByFilterAsync(
                filter.UserId,
                filter.ActivityType,
                filter.FromDate,
                filter.ToDate,
                filter.Limit,
                filter.Offset
            );
            return userActivities.Select(MapToDto).ToList();
        }

        public async Task<UserActivityDto> CreateUserActivityAsync(UserActivityCreateRequestDto request)
        {
            var userActivity = new Useractivity
            {
                UserId = request.UserId,
                ActivityType = request.ActivityType,
                ActivityDate = request.ActivityDate ?? DateTime.UtcNow,
                Metadata = ConvertStringToJson(request.Metadata)
            };

            var createdActivity = await _userActivityRepository.AddUserActivityAsync(userActivity);
            return MapToDto(createdActivity);
        }

        public async Task<UserActivityDto> UpdateUserActivityAsync(int activityId, UserActivityUpdateRequestDto request)
        {
            var existingActivity = await _userActivityRepository.GetUserActivityByIdAsync(activityId);
            if (existingActivity == null)
            {
                throw new NotFoundException($"UserActivity with ID {activityId} not found.");
            }

            existingActivity.UserId = request.UserId ?? existingActivity.UserId;
            existingActivity.ActivityType = request.ActivityType ?? existingActivity.ActivityType;
            existingActivity.ActivityDate = request.ActivityDate ?? existingActivity.ActivityDate;
            existingActivity.Metadata = request.Metadata != null ? ConvertStringToJson(request.Metadata) : existingActivity.Metadata;

            var updatedActivity = await _userActivityRepository.UpdateUserActivityAsync(existingActivity);
            return MapToDto(updatedActivity);
        }

        public async Task DeleteUserActivityAsync(int activityId)
        {
            var existingActivity = await _userActivityRepository.GetUserActivityByIdAsync(activityId);
            if (existingActivity == null)
            {
                throw new NotFoundException($"UserActivity with ID {activityId} not found.");
            }

            await _userActivityRepository.DeleteUserActivityAsync(activityId);
        }

        private static UserActivityDto MapToDto(Useractivity userActivity)
        {
            return new UserActivityDto
            {
                ActivityId = userActivity.ActivityId,
                UserId = userActivity.UserId,
                ActivityType = userActivity.ActivityType,
                ActivityDate = userActivity.ActivityDate,
                Metadata = userActivity.Metadata
            };
        }

        private static string? ConvertStringToJson(string? metadata)
        {
            if (string.IsNullOrEmpty(metadata))
                return null;

            try
            {
                // Try to parse as JSON first
                JsonDocument.Parse(metadata);
                return metadata; // Already valid JSON
            }
            catch
            {
                // If not valid JSON, wrap it in a JSON object
                var jsonObject = new { text = metadata };
                return JsonSerializer.Serialize(jsonObject);
            }
        }
    }
}
