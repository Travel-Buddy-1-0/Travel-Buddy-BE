using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly AppDbContext _context;

        public FavoriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Favorite?> GetByIdAsync(int favoriteId)
        {
            return await _context.Favorites
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.FavoriteId == favoriteId);
        }

        public async Task<IEnumerable<Favorite>> GetAllAsync()
        {
            return await _context.Favorites
                .Include(f => f.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Favorite>> GetByUserIdAsync(int userId)
        {
            return await _context.Favorites
                .Include(f => f.User)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Favorite>> GetByUserAndTargetAsync(int userId, string targetType, string targetId)
        {
            return await _context.Favorites
                .Include(f => f.User)
                .Where(f => f.UserId == userId && f.TargetType == targetType && f.TargetId == targetId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Favorite>> GetFilteredAsync(FavoriteFilterRequestDto filter)
        {
            var query = _context.Favorites
                .Include(f => f.User)
                .AsQueryable();

            // Apply filters
            if (filter.UserId.HasValue)
                query = query.Where(f => f.UserId == filter.UserId.Value);

            if (!string.IsNullOrEmpty(filter.TargetType))
                query = query.Where(f => f.TargetType == filter.TargetType);

            if (!string.IsNullOrEmpty(filter.TargetId))
                query = query.Where(f => f.TargetId == filter.TargetId);

            // Apply sorting
            query = filter.SortBy?.ToLower() switch
            {
                "favoriteid" => filter.SortOrder?.ToLower() == "asc" 
                    ? query.OrderBy(f => f.FavoriteId)
                    : query.OrderByDescending(f => f.FavoriteId),
                _ => filter.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(f => f.CreatedAt)
                    : query.OrderByDescending(f => f.CreatedAt)
            };

            // Apply pagination
            return await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<Favorite> CreateAsync(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return favorite;
        }

        public async Task<Favorite?> UpdateAsync(Favorite favorite)
        {
            var existingFavorite = await _context.Favorites.FindAsync(favorite.FavoriteId);
            if (existingFavorite == null)
                return null;

            existingFavorite.UserId = favorite.UserId;
            existingFavorite.TargetType = favorite.TargetType;
            existingFavorite.TargetId = favorite.TargetId;
            existingFavorite.CreatedAt = favorite.CreatedAt;

            await _context.SaveChangesAsync();
            return existingFavorite;
        }

        public async Task<bool> DeleteAsync(int favoriteId)
        {
            var favorite = await _context.Favorites.FindAsync(favoriteId);
            if (favorite == null)
                return false;

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int favoriteId)
        {
            return await _context.Favorites.AnyAsync(f => f.FavoriteId == favoriteId);
        }

        public async Task<bool> ExistsByUserAndTargetAsync(int userId, string targetType, string targetId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.TargetType == targetType && f.TargetId == targetId);
        }

        public async Task<int> GetTotalCountAsync(FavoriteFilterRequestDto filter)
        {
            var query = _context.Favorites.AsQueryable();

            // Apply same filters as GetFilteredAsync
            if (filter.UserId.HasValue)
                query = query.Where(f => f.UserId == filter.UserId.Value);

            if (!string.IsNullOrEmpty(filter.TargetType))
                query = query.Where(f => f.TargetType == filter.TargetType);

            if (!string.IsNullOrEmpty(filter.TargetId))
                query = query.Where(f => f.TargetId == filter.TargetId);

            return await query.CountAsync();
        }

        public async Task<Dictionary<string, Hotel>> GetHotelsByIdsAsync(IEnumerable<string> hotelIds)
        {
            var ids = hotelIds.Where(id => int.TryParse(id, out _)).Select(int.Parse).ToList();
            
            var hotels = await _context.Hotels
                .Where(h => ids.Contains(h.HotelId))
                .ToListAsync();
                
            return hotels.ToDictionary(h => h.HotelId.ToString(), h => h);
        }

        public async Task<Dictionary<string, Restaurant>> GetRestaurantsByIdsAsync(IEnumerable<string> restaurantIds)
        {
            var ids = restaurantIds.Where(id => int.TryParse(id, out _)).Select(int.Parse).ToList();
            
            var restaurants = await _context.Restaurants
                .Where(r => ids.Contains(r.RestaurantId))
                .ToListAsync();
                
            return restaurants.ToDictionary(r => r.RestaurantId.ToString(), r => r);
        }

        public async Task<Dictionary<string, Blog>> GetBlogsByIdsAsync(IEnumerable<string> blogIds)
        {
            var ids = blogIds.Where(id => int.TryParse(id, out _)).Select(int.Parse).ToList();
            
            var blogs = await _context.Blogs
                .Where(b => ids.Contains(b.BlogId))
                .ToListAsync();
                
            return blogs.ToDictionary(b => b.BlogId.ToString(), b => b);
        }
    }
}
