using BusinessObject.DTOs;
using BusinessObject.Models;

namespace Services
{
    public interface IBookingService
    {
        Task<HotelBookingResponseDto> CreateHotelBookingAsync(HotelBookingRequestDto bookingRequest);
        Task<HotelBookingResponseDto?> GetBookingByIdAsync(int bookingId);
        Task<HotelBookingResponseDto?> GetBookingByConfirmationNumberAsync(string confirmationNumber);
        Task<List<HotelBookingResponseDto>> GetBookingsByUserIdAsync(int userId);
        Task<List<HotelBookingResponseDto>> GetBookingsByHotelIdAsync(int hotelId);
        Task<HotelBookingResponseDto> UpdateBookingAsync(int bookingId, HotelBookingRequestDto bookingRequest);
        Task<bool> DeleteBookingAsync(int bookingId);
        Task<bool> ApproveBookingAsync(int bookingId);
        Task<bool> CancelBookingAsync(int bookingId);
        Task<List<HotelBookingResponseDto>> GetBookingsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
        Task<BookingConfirmationDto> GetBookingConfirmationAsync(int bookingId);
        Task<bool> IsHotelAvailableAsync(int hotelId, DateOnly checkInDate, DateOnly checkOutDate, int numberOfRooms);
    }
}
