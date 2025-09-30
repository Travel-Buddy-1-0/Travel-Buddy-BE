using BusinessObject.Data;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext _context;

        public HotelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Hotel>> GetSuggestionsAsync(int limit = 4)
        {
            return await _context.Hotels
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Hotel>> GetTopHotelsAsync(int limit = 4)
        {
            return await _context.Hotels
                .OrderByDescending(h => h.Reviews.Average(r => r.Rating))
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Hotel>> SearchHotelsAsync(string? location, int? guests, decimal? minPrice, decimal? maxPrice, string? type, int? stars, List<string>? amenities, int limit = 20, int offset = 0)
        {
            var query = _context.Hotels.AsQueryable();

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(h => h.Address != null && h.Address.Contains(location));
            }

            // Filter by style JSON properties
            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(h => h.Style != null && h.Style.Contains($"\"type\":\"{type}\""));
            }

            if (stars.HasValue)
            {
                query = query.Where(h => h.Style != null && h.Style.Contains($"\"stars\":{stars.Value}"));
            }

            if (amenities != null && amenities.Any())
            {
                foreach (var amenity in amenities)
                {
                    query = query.Where(h => h.Style != null && h.Style.Contains($"\"{amenity}\""));
                }
            }

            var hotels = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            // Filter by room price and capacity if specified
            if (minPrice.HasValue || maxPrice.HasValue || guests.HasValue)
            {
                var filtered = new List<Hotel>();
                foreach (var h in hotels)
                {
                    var rooms = await GetRoomsByHotelAsync(h.HotelId);
                    var candidateRooms = rooms.Where(r => 
                        (!minPrice.HasValue || r.PricePerNight >= minPrice) && 
                        (!maxPrice.HasValue || r.PricePerNight <= maxPrice) && 
                        (!guests.HasValue || r.Capacity >= guests));
                    if (candidateRooms.Any())
                        filtered.Add(h);
                }
                hotels = filtered;
            }

            return hotels;
        }

        public async Task<Hotel?> GetByIdAsync(int hotelId)
        {
            return await _context.Hotels
                .FirstOrDefaultAsync(h => h.HotelId == hotelId);
        }

        public async Task<List<Room>> GetRoomsByHotelAsync(int hotelId)
        {
            return await _context.Rooms.Include(x => x.Bookingdetails)
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();
        }

        public async Task<decimal?> GetAverageRatingAsync(int hotelId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.HotelId == hotelId && r.Rating.HasValue)
                .Select(r => r.Rating!.Value)
                .ToListAsync();

            if (!ratings.Any()) return null;
            return (decimal?)ratings.Average();
        }

        public async Task<List<Review>> GetReviewsByHotelAsync(int hotelId)
        {
            return await _context.Reviews
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();
        }

        public async Task<Bookingdetail> CreateBookingAsync(Bookingdetail detail)
        {
            _context.Bookingdetails.Add(detail);
            await _context.SaveChangesAsync();
            return detail;
        }

        public async Task<List<Bookingdetail>> GetBookingHistoryAsync(int userId, DateOnly? bookingDate)
        {
            var query = _context.Bookingdetails
                .Where(b => b.UserId == userId);

            if (bookingDate.HasValue)
            {
                var date = bookingDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(b => b.BookingDate.HasValue && 
                    b.BookingDate.Value.Date == date.Date);
            }

            return await query.ToListAsync();
        }
    }
}