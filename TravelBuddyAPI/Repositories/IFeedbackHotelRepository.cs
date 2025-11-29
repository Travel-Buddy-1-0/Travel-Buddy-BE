using BusinessObject.Entities;

namespace Repositories
{
    public interface IFeedbackHotelRepository
    {
        Task<List<FeedbackHotel>> GetAllFeedbackHotelsAsync();
        Task<FeedbackHotel?> GetFeedbackHotelByIdAsync(int feedbackId);
        Task<List<FeedbackHotel>> GetFeedbackHotelsByUserIdAsync(int userId);
        Task<List<FeedbackHotel>> GetFeedbackHotelsByHotelIdAsync(int hotelId);
        Task<List<FeedbackHotel>> GetFeedbackHotelsByFilterAsync(int? userId, int? hotelId, int? rating, DateTime? fromDate, DateTime? toDate, int limit, int offset);
        Task<FeedbackHotel> AddFeedbackHotelAsync(FeedbackHotel feedbackHotel);
        Task<FeedbackHotel> UpdateFeedbackHotelAsync(FeedbackHotel feedbackHotel);
        Task DeleteFeedbackHotelAsync(int feedbackId);
        Task<bool> ExistsFeedbackByUserAndHotelAsync(int userId, int hotelId);
    }
}
