using BusinessObject.Models;
using Supabase.Postgrest;
using static Supabase.Postgrest.Constants;

namespace Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly Supabase.Client _supabase;

        public HotelRepository(Supabase.Client supabaseClient)
        {
            _supabase = supabaseClient;
        }

        public async Task<List<Hotel>> GetSuggestionsAsync(int limit = 4)
        {
            var response = await _supabase.From<Hotel>().Limit(limit).Get();
            return response.Models;
        }

        public async Task<List<Hotel>> GetTopHotelsAsync(int limit)
        {
            // Assuming top by average rating in review table
            // As Postgrest client aggregation is limited, fallback to simple order by name for now
            var response = await _supabase.From<Hotel>().Limit(limit).Get();
            return response.Models;
        }

        public async Task<List<Hotel>> SearchHotelsAsync(string? location, int? guests, decimal? minPrice, decimal? maxPrice, string? type, int limit = 20, int offset = 0)
        {
            var query = _supabase.From<Hotel>() as Table<Hotel>;

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = (Table<Hotel>?)query.Filter("address", Operator.ILike, $"%{location}%");
            }
            if (!string.IsNullOrWhiteSpace(type))
            {
                query = (Table<Hotel>?)query.Filter("style", Operator.ILike, $"%{type}%");
            }

            var response = await query.Range(offset, offset + limit - 1).Get();
            var hotels = response.Models;

            // Basic price filter by checking min room price per hotel if rooms available
            if (minPrice.HasValue || maxPrice.HasValue || guests.HasValue)
            {
                var filtered = new List<Hotel>();
                foreach (var h in hotels)
                {
                    var rooms = await GetRoomsByHotelAsync(h.HotelId);
                    var candidateRooms = rooms.Where(r => (!minPrice.HasValue || r.PricePerNight >= minPrice) && (!maxPrice.HasValue || r.PricePerNight <= maxPrice) && (!guests.HasValue || r.Capacity >= guests));
                    if (candidateRooms.Any())
                        filtered.Add(h);
                }
                hotels = filtered;
            }

            return hotels;
        }

        public async Task<Hotel?> GetByIdAsync(int hotelId)
        {
            var response = await _supabase.From<Hotel>().Filter("hotel_id", Operator.Equals, hotelId).Single();
            return response;
        }

        public async Task<List<Room>> GetRoomsByHotelAsync(int hotelId)
        {
            var response = await _supabase.From<Room>().Filter("hotel_id", Operator.Equals, hotelId).Get();
            return response.Models;
        }

        public async Task<decimal?> GetAverageRatingAsync(int hotelId)
        {
            // Postgrest .NET client lacks direct AVG; fetch and compute client-side
            var response = await _supabase.From<Review>().Filter("hotel_id", Operator.Equals, hotelId).Get();
            var ratings = response.Models.Where(r => r.Rating.HasValue).Select(r => (decimal)r.Rating!.Value);
            if (!ratings.Any()) return null;
            return ratings.Average();
        }

        public async Task<List<Review>> GetReviewsByHotelAsync(int hotelId)
        {
            var response = await _supabase.From<Review>().Filter("hotel_id", Operator.Equals, hotelId).Get();
            return response.Models;
        }

        public async Task<BookingDetail> CreateBookingAsync(BookingDetail detail)
        {
            var inserted = await _supabase.From<BookingDetail>().Insert(detail);
            return inserted.Models.First();
        }

        public async Task<List<BookingDetail>> GetBookingHistoryAsync(int userId, DateOnly? bookingDate)
        {
            var query = _supabase.From<BookingDetail>().Filter("user_id", Operator.Equals, userId);
            if (bookingDate.HasValue)
            {
                query = query.Filter("booking_date", Operator.Equals, bookingDate.Value.ToString("yyyy-MM-dd"));
            }
            var response = await query.Get();
            return response.Models;
        }
    }
}


