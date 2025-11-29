using BusinessObject.DTOs;
using BusinessObject.Entities;

namespace Repositories
{
    public interface IFavoriteRepository
    {
        Task<Favorite?> GetByIdAsync(int favoriteId);
        Task<IEnumerable<Favorite>> GetAllAsync();
        Task<IEnumerable<Favorite>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Favorite>> GetByUserAndTargetAsync(int userId, string targetType, string targetId);
        Task<IEnumerable<Favorite>> GetFilteredAsync(FavoriteFilterRequestDto filter);
        Task<Favorite> CreateAsync(Favorite favorite);
        Task<Favorite?> UpdateAsync(Favorite favorite);
        Task<bool> DeleteAsync(int favoriteId);
        Task<bool> ExistsAsync(int favoriteId);
        Task<bool> ExistsByUserAndTargetAsync(int userId, string targetType, string targetId);
        Task<int> GetTotalCountAsync(FavoriteFilterRequestDto filter);
        
        // New methods for getting related data
        Task<Dictionary<string, Hotel>> GetHotelsByIdsAsync(IEnumerable<string> hotelIds);
        Task<Dictionary<string, Restaurant>> GetRestaurantsByIdsAsync(IEnumerable<string> restaurantIds);
        Task<Dictionary<string, Blog>> GetBlogsByIdsAsync(IEnumerable<string> blogIds);
    }
}
