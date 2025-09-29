using BusinessObject.Models;
using Supabase;
using Supabase.Postgrest.Models;

namespace Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly Supabase.Client _supabase;

        public BookingRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<BookingDetail> CreateBookingAsync(BookingDetail booking)
        {
            try
            {
                // Generate confirmation number
                booking.ConfirmationNumber = GenerateConfirmationNumber();
                
                var response = await _supabase
                    .From<BookingDetail>()
                    .Insert(booking);

                return response.Models.FirstOrDefault() ?? throw new Exception("Failed to create booking");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating booking: {ex.Message}", ex);
            }
        }

        public async Task<BookingDetail?> GetBookingByIdAsync(int bookingId)
        {
            try
            {
                var response = await _supabase
                    .From<BookingDetail>()
                    .Where(x => x.BookingId == bookingId)
                    .Get();

                return response.Models.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting booking by ID: {ex.Message}", ex);
            }
        }

        public async Task<BookingDetail?> GetBookingByConfirmationNumberAsync(string confirmationNumber)
        {
            try
            {
                var response = await _supabase
                    .From<BookingDetail>()
                    .Where(x => x.ConfirmationNumber == confirmationNumber)
                    .Get();

                return response.Models.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting booking by confirmation number: {ex.Message}", ex);
            }
        }

        public async Task<List<BookingDetail>> GetBookingsByUserIdAsync(int userId)
        {
            try
            {
                var response = await _supabase
                    .From<BookingDetail>()
                    .Where(x => x.UserId == userId)
                    .Order(x => x.BookingDate, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting bookings by user ID: {ex.Message}", ex);
            }
        }

        public async Task<List<BookingDetail>> GetBookingsByHotelIdAsync(int hotelId)
        {
            try
            {
                var response = await _supabase
                    .From<BookingDetail>()
                    .Where(x => x.HotelId == hotelId)
                    .Order(x => x.BookingDate, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting bookings by hotel ID: {ex.Message}", ex);
            }
        }

        public async Task<BookingDetail> UpdateBookingAsync(BookingDetail booking)
        {
            try
            {
                var response = await _supabase
                    .From<BookingDetail>()
                    .Where(x => x.BookingId == booking.BookingId)
                    .Update(booking);

                return response.Models.FirstOrDefault() ?? throw new Exception("Failed to update booking");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating booking: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            try
            {
                await _supabase
                    .From<BookingDetail>()
                    .Where(x => x.BookingId == bookingId)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting booking: {ex.Message}", ex);
            }
        }

        public async Task<bool> ApproveBookingAsync(int bookingId)
        {
            try
            {
                var response = await _supabase
                    .From<BookingDetail>()
                    .Where(x => x.BookingId == bookingId)
                    .Set(x => x.Approved, true)
                    .Set(x => x.BookingStatus, "Confirmed")
                    .Update();

                return response.Models.Any();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error approving booking: {ex.Message}", ex);
            }
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            try
            {
                var response = await _supabase
                    .From<BookingDetail>()
                    .Where(x => x.BookingId == bookingId)
                    .Set(x => x.BookingStatus, "Cancelled")
                    .Update();

                return response.Models.Any();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error cancelling booking: {ex.Message}", ex);
            }
        }

        public async Task<List<BookingDetail>> GetBookingsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                var response = await _supabase
                    .From<BookingDetail>()
                    .Where(x => x.CheckInDate >= startDate && x.CheckOutDate <= endDate)
                    .Order(x => x.CheckInDate, Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                return response.Models;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting bookings by date range: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsHotelAvailableAsync(int hotelId, DateOnly checkInDate, DateOnly checkOutDate, int numberOfRooms)
        {
            try
            {
                // Check for overlapping bookings
                var response = await _supabase
                    .From<BookingDetail>()
                    .Where(x => x.HotelId == hotelId && 
                               x.BookingStatus != "Cancelled" &&
                               ((x.CheckInDate <= checkInDate && x.CheckOutDate > checkInDate) ||
                                (x.CheckInDate < checkOutDate && x.CheckOutDate >= checkOutDate) ||
                                (x.CheckInDate >= checkInDate && x.CheckOutDate <= checkOutDate)))
                    .Get();

                // For simplicity, we'll assume each booking takes 1 room
                // In a real application, you'd need to check room availability more carefully
                var totalBookedRooms = response.Models.Sum(x => x.NumberOfRooms ?? 0);
                
                // Assume hotel has 100 rooms available (this should come from hotel configuration)
                const int totalAvailableRooms = 100;
                
                return (totalBookedRooms + numberOfRooms) <= totalAvailableRooms;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking hotel availability: {ex.Message}", ex);
            }
        }

        private string GenerateConfirmationNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"TB{timestamp}{random}";
        }
    }
}
