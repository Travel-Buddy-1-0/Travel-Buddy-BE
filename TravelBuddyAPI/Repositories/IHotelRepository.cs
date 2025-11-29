using BusinessObject.Entities;

namespace Repositories
{
    public interface IHotelRepository
    {
        Task<List<Hotel>> GetSuggestionsAsync(int limit = 4);
        Task<List<Hotel>> GetTopHotelsAsync(int limit);
        Task<List<Hotel>> SearchHotelsAsync(string? location, int? guests, decimal? minPrice, decimal? maxPrice, string? type, int? stars, List<string>? amenities, int limit = 20, int offset = 0);
        Task<Hotel?> GetByIdAsync(int hotelId);
        Task<List<Room>> GetRoomsByHotelAsync(int hotelId);
        Task<decimal?> GetAverageRatingAsync(int hotelId);
        Task<List<Review>> GetReviewsByHotelAsync(int hotelId, int? rating = null, int limit = 20, int offset = 0);
        Task<Bookingdetail> CreateBookingAsync(Bookingdetail detail);
        Task<List<Bookingdetail>> GetBookingHistoryAsync(int userId, DateOnly? bookingDate);

        Task<int> ChangeStatusBookingAsync(int bookingId, int status);
    }
}


