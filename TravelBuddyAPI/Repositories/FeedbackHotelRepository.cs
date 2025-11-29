using BusinessObject.Data;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class FeedbackHotelRepository : IFeedbackHotelRepository
    {
        private readonly AppDbContext _context;

        public FeedbackHotelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FeedbackHotel>> GetAllFeedbackHotelsAsync()
        {
            return await _context.FeedbackHotels
                .Include(fh => fh.User)
                .Include(fh => fh.Hotel)
                .OrderByDescending(fh => fh.CreatedAt)
                .ToListAsync();
        }

        public async Task<FeedbackHotel?> GetFeedbackHotelByIdAsync(int feedbackId)
        {
            return await _context.FeedbackHotels
                .Include(fh => fh.User)
                .Include(fh => fh.Hotel)
                .FirstOrDefaultAsync(fh => fh.FeedbackId == feedbackId);
        }

        public async Task<List<FeedbackHotel>> GetFeedbackHotelsByUserIdAsync(int userId)
        {
            return await _context.FeedbackHotels
                .Include(fh => fh.User)
                .Include(fh => fh.Hotel)
                .Where(fh => fh.UserId == userId)
                .OrderByDescending(fh => fh.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<FeedbackHotel>> GetFeedbackHotelsByHotelIdAsync(int hotelId)
        {
            return await _context.FeedbackHotels
                .Include(fh => fh.User)
                .Include(fh => fh.Hotel)
                .Where(fh => fh.HotelId == hotelId)
                .OrderByDescending(fh => fh.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<FeedbackHotel>> GetFeedbackHotelsByFilterAsync(int? userId, int? hotelId, int? rating, DateTime? fromDate, DateTime? toDate, int limit, int offset)
        {
            var query = _context.FeedbackHotels
                .Include(fh => fh.User)
                .Include(fh => fh.Hotel)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(fh => fh.UserId == userId.Value);
            }

            if (hotelId.HasValue)
            {
                query = query.Where(fh => fh.HotelId == hotelId.Value);
            }

            if (rating.HasValue)
            {
                query = query.Where(fh => fh.Rating == rating.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(fh => fh.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(fh => fh.CreatedAt <= toDate.Value);
            }

            return await query
                .OrderByDescending(fh => fh.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<FeedbackHotel> AddFeedbackHotelAsync(FeedbackHotel feedbackHotel)
        {
            feedbackHotel.CreatedAt = DateTime.UtcNow;
            _context.FeedbackHotels.Add(feedbackHotel);
            await _context.SaveChangesAsync();
            return feedbackHotel;
        }

        public async Task<FeedbackHotel> UpdateFeedbackHotelAsync(FeedbackHotel feedbackHotel)
        {
            feedbackHotel.UpdatedAt = DateTime.UtcNow;
            _context.FeedbackHotels.Update(feedbackHotel);
            await _context.SaveChangesAsync();
            return feedbackHotel;
        }

        public async Task DeleteFeedbackHotelAsync(int feedbackId)
        {
            var feedbackHotel = await _context.FeedbackHotels.FindAsync(feedbackId);
            if (feedbackHotel != null)
            {
                _context.FeedbackHotels.Remove(feedbackHotel);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsFeedbackByUserAndHotelAsync(int userId, int hotelId)
        {
            return await _context.FeedbackHotels
                .AnyAsync(fh => fh.UserId == userId && fh.HotelId == hotelId);
        }
    }
}
