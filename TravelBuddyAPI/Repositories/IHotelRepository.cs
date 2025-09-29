using BusinessObject.Models;

namespace Repositories
{
    public interface IHotelRepository
    {
        Task<List<Hotel>> GetAllHotelsAsync();
        Task<Hotel?> GetHotelByIdAsync(int hotelId);
        Task<List<Hotel>> GetSuggestedHotelsAsync(int count = 4);
        Task<List<Hotel>> GetTopHotelsAsync(int count = 4);
        Task<List<Hotel>> SearchHotelsAsync(string? location, DateTime? checkIn, DateTime? checkOut, int? guests);
        Task<List<Hotel>> FilterHotelsAsync(string? style, string? location);
        Task<List<Room>> GetHotelRoomsAsync(int hotelId);
        Task<Hotel> AddHotelAsync(Hotel hotel);
        Task<Hotel> UpdateHotelAsync(Hotel hotel);
        Task DeleteHotelAsync(int hotelId);
    }
}
