using BusinessObject.Models;

namespace Repositories
{
    public interface IBookingRepository
    {
        Task<BookingDetail> CreateBookingAsync(BookingDetail booking);
        Task<BookingDetail?> GetBookingByIdAsync(int bookingId);
        Task<BookingDetail?> GetBookingByConfirmationNumberAsync(string confirmationNumber);
        Task<List<BookingDetail>> GetBookingsByUserIdAsync(int userId);
        Task<List<BookingDetail>> GetBookingsByHotelIdAsync(int hotelId);
        Task<BookingDetail> UpdateBookingAsync(BookingDetail booking);
        Task<bool> DeleteBookingAsync(int bookingId);
        Task<bool> ApproveBookingAsync(int bookingId);
        Task<bool> CancelBookingAsync(int bookingId);
        Task<List<BookingDetail>> GetBookingsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
        Task<bool> IsHotelAvailableAsync(int hotelId, DateOnly checkInDate, DateOnly checkOutDate, int numberOfRooms);
    }
}
