using BusinessObject.DTOs;
using BusinessObject.Entities;

namespace BusinessLogic.Services
{
    public interface IFeedbackHotelService
    {
        Task<List<FeedbackHotelDto>> GetAllFeedbackHotelsAsync();
        Task<FeedbackHotelDto?> GetFeedbackHotelByIdAsync(int feedbackId);
        Task<List<FeedbackHotelDto>> GetFeedbackHotelsByUserIdAsync(int userId);
        Task<List<FeedbackHotelDto>> GetFeedbackHotelsByHotelIdAsync(int hotelId);
        Task<List<FeedbackHotelDto>> GetFeedbackHotelsByFilterAsync(FeedbackHotelFilterRequestDto filter);
        Task<FeedbackHotelDto> CreateFeedbackHotelAsync(FeedbackHotelCreateRequestDto request);
        Task<FeedbackHotelDto> UpdateFeedbackHotelAsync(int feedbackId, FeedbackHotelUpdateRequestDto request);
        Task DeleteFeedbackHotelAsync(int feedbackId);
        Task<bool> CanUserCreateFeedbackAsync(int userId, int hotelId);
    }
}
