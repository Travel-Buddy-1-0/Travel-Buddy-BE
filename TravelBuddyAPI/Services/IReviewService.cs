using BusinessObject.DTOs;
using BusinessObject.Entities;

namespace BusinessLogic.Services
{
    public interface IReviewService
    {
        Task<List<ReviewDto>> GetAllReviewsAsync();
        Task<ReviewDto?> GetReviewByIdAsync(int reviewId);
        Task<List<ReviewDto>> GetReviewsByUserIdAsync(int userId);
        Task<List<ReviewDto>> GetReviewsByHotelIdAsync(int hotelId);
        Task<List<ReviewDto>> GetReviewsByRestaurantIdAsync(int restaurantId);
        Task<List<ReviewDto>> GetReviewsByTourIdAsync(int tourId);
        Task<List<ReviewDto>> GetReviewsByFilterAsync(ReviewFilterRequestDto filter);
        Task<ReviewDto> CreateReviewAsync(ReviewCreateRequestDto request);
        Task<ReviewDto> UpdateReviewAsync(int reviewId, ReviewUpdateRequestDto request);
        Task DeleteReviewAsync(int reviewId);
        Task<bool> CanUserCreateReviewAsync(int userId, int? tourId, int? hotelId, int? restaurantId);
    }
}
