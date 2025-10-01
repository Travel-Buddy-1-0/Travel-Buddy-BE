using BusinessObject.Data;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Room?> GetByIdAsync(int roomId)
        {
            return await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
        }

        public async Task<Room?> GetByIdWithHotelAsync(int roomId)
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
        }

        public async Task<DateOnly?> GetLastCheckoutDateAsync(int roomId)
        {
            var lastCheckout = await _context.Bookingdetails
                .Where(b => b.RoomId == roomId && b.CheckOutDate.HasValue)
                .OrderByDescending(b => b.CheckOutDate)
                .Select(b => b.CheckOutDate)
                .FirstOrDefaultAsync();

            return lastCheckout;
        }
    }
}
