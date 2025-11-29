using BusinessObject.DTOs;

namespace Services
{
    public interface IFavoriteService
    {
        Task<FavoriteDto?> GetByIdAsync(int favoriteId);
        Task<IEnumerable<FavoriteDto>> GetAllAsync();
        Task<IEnumerable<FavoriteDto>> GetByUserIdAsync(int userId);
        Task<FavoriteDto?> CreateAsync(FavoriteCreateRequestDto request);
        Task<FavoriteDto?> UpdateAsync(int favoriteId, FavoriteCreateRequestDto request);
        Task<bool> DeleteAsync(int favoriteId);
        Task<IEnumerable<FavoriteDto>> GetFilteredAsync(FavoriteFilterRequestDto filter);
        Task<bool> IsFavoriteAsync(int userId, string targetType, string targetId);
        Task<bool> ToggleFavoriteAsync(FavoriteCreateRequestDto request);
        Task<int> GetTotalCountAsync(FavoriteFilterRequestDto filter);
    }
}
