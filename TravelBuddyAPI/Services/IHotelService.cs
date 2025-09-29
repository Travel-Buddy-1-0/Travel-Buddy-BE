using BusinessObject.DTOs;
using BusinessObject.Models;

namespace BusinessLogic.Services
{
    public interface IHotelService
    {
        Task<List<HotelResponseDto>> GetSuggestedHotelsAsync();
        Task<List<HotelResponseDto>> GetTopHotelsAsync();
        Task<List<HotelResponseDto>> SearchHotelsAsync(HotelSearchRequestDto searchRequest);
        Task<List<HotelResponseDto>> FilterHotelsAsync(HotelFilterRequestDto filterRequest);
        Task<HotelResponseDto> GetHotelDetailAsync(int hotelId);
        Task<List<Hotel>> GetAllHotelsAsync();
        Task<Hotel> CreateHotelAsync(Hotel hotel);
        Task<Hotel> UpdateHotelAsync(Hotel hotel);
        Task DeleteHotelAsync(int hotelId);
    }
}
