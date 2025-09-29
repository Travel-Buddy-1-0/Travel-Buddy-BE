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

        public async Task<List<Hotel>> GetAllHotelsAsync()
        {
            var response = await _supabase.From<Hotel>().Get();
            return response.Models;
        }

        public async Task<Hotel?> GetHotelByIdAsync(int hotelId)
        {
            var response = await _supabase
                .From<Hotel>()
                .Filter("hotel_id", Operator.Equals, hotelId)
                .Single();
            return response;
        }

        public async Task<List<Hotel>> GetSuggestedHotelsAsync(int count = 4)
        {
            // Get all hotels
            var response = await _supabase.From<Hotel>().Get();
            return response.Models
                .Take(count)
                .ToList();
        }

        public async Task<List<Hotel>> GetTopHotelsAsync(int count = 4)
        {
            // Get all hotels
            var response = await _supabase.From<Hotel>().Get();
            return response.Models
                .Take(count)
                .ToList();
        }

        public async Task<List<Hotel>> SearchHotelsAsync(string? location, DateTime? checkIn, DateTime? checkOut, int? guests)
        {
            // Get all hotels and filter in memory
            var response = await _supabase.From<Hotel>().Get();
            var hotels = response.Models.AsQueryable();

            if (!string.IsNullOrEmpty(location))
            {
                hotels = hotels.Where(h => h.Address != null && h.Address.Contains(location, StringComparison.OrdinalIgnoreCase));
            }

            return hotels.ToList();
        }

        public async Task<List<Hotel>> FilterHotelsAsync(string? style, string? location)
        {
            // Start with base query
            var response = await _supabase.From<Hotel>().Get();
            var hotels = response.Models.AsQueryable();

            // Apply filters in memory
            if (!string.IsNullOrEmpty(style))
            {
                hotels = hotels.Where(h => h.Style != null && h.Style.ToString().Contains(style, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(location))
            {
                hotels = hotels.Where(h => h.Address != null && h.Address.Contains(location, StringComparison.OrdinalIgnoreCase));
            }

            return hotels.ToList();
        }

        public async Task<List<Room>> GetHotelRoomsAsync(int hotelId)
        {
            var response = await _supabase
                .From<Room>()
                .Filter("hotel_id", Operator.Equals, hotelId)
                .Get();
            return response.Models;
        }

        public async Task<Hotel> AddHotelAsync(Hotel hotel)
        {
            var response = await _supabase.From<Hotel>().Insert(hotel);
            return response.Models.First();
        }

        public async Task<Hotel> UpdateHotelAsync(Hotel hotel)
        {
            var response = await _supabase.From<Hotel>().Update(hotel);
            return response.Models.First();
        }

        public async Task DeleteHotelAsync(int hotelId)
        {
            await _supabase
                .From<Hotel>()
                .Filter("hotel_id", Operator.Equals, hotelId)
                .Delete();
        }
    }
}
