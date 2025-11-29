using BusinessObject.Data;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Hotel)
                .Include(r => r.Restaurant)
                .Include(r => r.Tour)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Hotel)
                .Include(r => r.Restaurant)
                .Include(r => r.Tour)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }

        public async Task<List<Review>> GetReviewsByUserIdAsync(int userId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Hotel)
                .Include(r => r.Restaurant)
                .Include(r => r.Tour)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByHotelIdAsync(int hotelId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Hotel)
                .Include(r => r.Restaurant)
                .Include(r => r.Tour)
                .Where(r => r.HotelId == hotelId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByRestaurantIdAsync(int restaurantId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Hotel)
                .Include(r => r.Restaurant)
                .Include(r => r.Tour)
                .Where(r => r.RestaurantId == restaurantId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByTourIdAsync(int tourId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Hotel)
                .Include(r => r.Restaurant)
                .Include(r => r.Tour)
                .Where(r => r.TourId == tourId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByFilterAsync(int? userId, int? tourId, int? hotelId, int? restaurantId, int? rating, DateTime? fromDate, DateTime? toDate, int limit, int offset)
        {
            var query = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Hotel)
                .Include(r => r.Restaurant)
                .Include(r => r.Tour)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(r => r.UserId == userId.Value);
            }

            if (tourId.HasValue)
            {
                query = query.Where(r => r.TourId == tourId.Value);
            }

            if (hotelId.HasValue)
            {
                query = query.Where(r => r.HotelId == hotelId.Value);
            }

            if (restaurantId.HasValue)
            {
                query = query.Where(r => r.RestaurantId == restaurantId.Value);
            }

            if (rating.HasValue)
            {
                query = query.Where(r => r.Rating == rating.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(r => r.ReviewDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(r => r.ReviewDate <= toDate.Value);
            }

            return await query
                .OrderByDescending(r => r.ReviewDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            review.ReviewDate = DateTime.UtcNow;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsReviewByUserAndEntityAsync(int userId, int? tourId, int? hotelId, int? restaurantId)
        {
            var query = _context.Reviews.Where(r => r.UserId == userId);

            if (tourId.HasValue)
            {
                query = query.Where(r => r.TourId == tourId.Value);
            }
            else if (hotelId.HasValue)
            {
                query = query.Where(r => r.HotelId == hotelId.Value);
            }
            else if (restaurantId.HasValue)
            {
                query = query.Where(r => r.RestaurantId == restaurantId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
