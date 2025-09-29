using BusinessObject.Models;

namespace Repositories
{
    public interface IHotelRepository
    {
        Task<List<Hotel>> GetSuggestionsAsync(int limit = 4);
        Task<List<Hotel>> GetTopHotelsAsync(int limit = 4);
        Task<List<Hotel>> SearchHotelsAsync(string? location, int? guests, decimal? minPrice, decimal? maxPrice, string? type, int limit = 20, int offset = 0);
        Task<Hotel?> GetByIdAsync(int hotelId);
        Task<List<Room>> GetRoomsByHotelAsync(int hotelId);
        Task<decimal?> GetAverageRatingAsync(int hotelId);
        Task<List<Review>> GetReviewsByHotelAsync(int hotelId);
        Task<BookingDetail> CreateBookingAsync(BookingDetail detail);
    }
}


