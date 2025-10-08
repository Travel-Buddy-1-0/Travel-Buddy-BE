using BusinessObject.Entities;

namespace Repositories
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllReviewsAsync();
        Task<Review?> GetReviewByIdAsync(int reviewId);
        Task<List<Review>> GetReviewsByUserIdAsync(int userId);
        Task<List<Review>> GetReviewsByHotelIdAsync(int hotelId);
        Task<List<Review>> GetReviewsByRestaurantIdAsync(int restaurantId);
        Task<List<Review>> GetReviewsByTourIdAsync(int tourId);
        Task<List<Review>> GetReviewsByFilterAsync(int? userId, int? tourId, int? hotelId, int? restaurantId, int? rating, DateTime? fromDate, DateTime? toDate, int limit, int offset);
        Task<Review> AddReviewAsync(Review review);
        Task<Review> UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int reviewId);
        Task<bool> ExistsReviewByUserAndEntityAsync(int userId, int? tourId, int? hotelId, int? restaurantId);
    }
}
