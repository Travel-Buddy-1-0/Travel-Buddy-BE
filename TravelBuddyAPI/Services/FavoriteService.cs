using BusinessLogic.Exceptions;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using Repositories;

namespace Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<FavoriteDto?> GetByIdAsync(int favoriteId)
        {
            var favorite = await _favoriteRepository.GetByIdAsync(favoriteId);
            return favorite != null ? MapToDto(favorite) : null;
        }

        public async Task<IEnumerable<FavoriteDto>> GetAllAsync()
        {
            var favorites = await _favoriteRepository.GetAllAsync();
            return favorites.Select(MapToDto);
        }

        public async Task<IEnumerable<FavoriteDto>> GetByUserIdAsync(int userId)
        {
            var favorites = await _favoriteRepository.GetByUserIdAsync(userId);
            return await MapToDtosWithRelatedDataAsync(favorites);
        }

        public async Task<FavoriteDto?> CreateAsync(FavoriteCreateRequestDto request)
        {
            // Check if already exists
            var exists = await _favoriteRepository.ExistsByUserAndTargetAsync(request.UserId, request.TargetType, request.TargetId);
            if (exists)
            {
                throw new Exception($"Favorite already exists for user {request.UserId} and target {request.TargetType}:{request.TargetId}");
            }

            var favorite = new Favorite
            {
                UserId = request.UserId,
                TargetType = request.TargetType,
                TargetId = request.TargetId,
                CreatedAt = DateTime.UtcNow
            };

            var createdFavorite = await _favoriteRepository.CreateAsync(favorite);
            return MapToDto(createdFavorite);
        }

        public async Task<FavoriteDto?> UpdateAsync(int favoriteId, FavoriteCreateRequestDto request)
        {
            var favorite = await _favoriteRepository.GetByIdAsync(favoriteId);
            if (favorite == null)
            {
                throw new NotFoundException($"Favorite with ID {favoriteId} not found");
            }

            // Check if the new combination already exists (excluding current favorite)
            var exists = await _favoriteRepository.ExistsByUserAndTargetAsync(request.UserId, request.TargetType, request.TargetId);
            if (exists)
            {
                var existingFavorites = await _favoriteRepository.GetByUserAndTargetAsync(request.UserId, request.TargetType, request.TargetId);
                if (existingFavorites.Any(f => f.FavoriteId != favoriteId))
                {
                    throw new ConflictException($"Favorite already exists for user {request.UserId} and target {request.TargetType}:{request.TargetId}");
                }
            }

            favorite.UserId = request.UserId;
            favorite.TargetType = request.TargetType;
            favorite.TargetId = request.TargetId;

            var updatedFavorite = await _favoriteRepository.UpdateAsync(favorite);
            return updatedFavorite != null ? MapToDto(updatedFavorite) : null;
        }

        public async Task<bool> DeleteAsync(int favoriteId)
        {
            return await _favoriteRepository.DeleteAsync(favoriteId);
        }

        public async Task<IEnumerable<FavoriteDto>> GetFilteredAsync(FavoriteFilterRequestDto filter)
        {
            var favorites = await _favoriteRepository.GetFilteredAsync(filter);
            return await MapToDtosWithRelatedDataAsync(favorites);
        }

        public async Task<bool> IsFavoriteAsync(int userId, string targetType, string targetId)
        {
            return await _favoriteRepository.ExistsByUserAndTargetAsync(userId, targetType, targetId);
        }

        public async Task<bool> ToggleFavoriteAsync(FavoriteCreateRequestDto request)
        {
            var exists = await _favoriteRepository.ExistsByUserAndTargetAsync(request.UserId, request.TargetType, request.TargetId);
            
            if (exists)
            {
                // Remove favorite
                var favorites = await _favoriteRepository.GetByUserAndTargetAsync(request.UserId, request.TargetType, request.TargetId);
                var favorite = favorites.FirstOrDefault();
                if (favorite != null)
                {
                    return await _favoriteRepository.DeleteAsync(favorite.FavoriteId);
                }
                return false;
            }
            else
            {
                // Add favorite
                var favorite = new Favorite
                {
                    UserId = request.UserId,
                    TargetType = request.TargetType,
                    TargetId = request.TargetId,
                    CreatedAt = DateTime.UtcNow
                };
                var createdFavorite = await _favoriteRepository.CreateAsync(favorite);
                return createdFavorite != null;
            }
        }

        public async Task<int> GetTotalCountAsync(FavoriteFilterRequestDto filter)
        {
            return await _favoriteRepository.GetTotalCountAsync(filter);
        }

        private async Task<IEnumerable<FavoriteDto>> MapToDtosWithRelatedDataAsync(IEnumerable<Favorite> favorites)
        {
            var favoriteList = favorites.ToList();
            
            // Group favorites by target type
            var hotelIds = favoriteList.Where(f => f.TargetType == "HOTEL").Select(f => f.TargetId).Distinct();
            var restaurantIds = favoriteList.Where(f => f.TargetType == "RESTAURANT").Select(f => f.TargetId).Distinct();
            var blogIds = favoriteList.Where(f => f.TargetType == "POST").Select(f => f.TargetId).Distinct();

            // Fetch related data in parallel
            var hotelsTask = hotelIds.Any() ? _favoriteRepository.GetHotelsByIdsAsync(hotelIds) : Task.FromResult(new Dictionary<string, Hotel>());
            var restaurantsTask = restaurantIds.Any() ? _favoriteRepository.GetRestaurantsByIdsAsync(restaurantIds) : Task.FromResult(new Dictionary<string, Restaurant>());
            var blogsTask = blogIds.Any() ? _favoriteRepository.GetBlogsByIdsAsync(blogIds) : Task.FromResult(new Dictionary<string, Blog>());

            await Task.WhenAll(hotelsTask, restaurantsTask, blogsTask);

            var hotels = await hotelsTask;
            var restaurants = await restaurantsTask;
            var blogs = await blogsTask;

            // Map to DTOs with related data
            return favoriteList.Select(favorite => MapToDtoWithRelatedData(favorite, hotels, restaurants, blogs));
        }

        private FavoriteDto MapToDto(Favorite favorite)
        {
            return new FavoriteDto
            {
                FavoriteId = favorite.FavoriteId,
                UserId = favorite.UserId,
                TargetType = favorite.TargetType,
                TargetId = favorite.TargetId,
                CreatedAt = favorite.CreatedAt,
                Hotel = null,
                Restaurant = null,
                Post = null
            };
        }

        private FavoriteDto MapToDtoWithRelatedData(Favorite favorite, 
            Dictionary<string, Hotel> hotels, 
            Dictionary<string, Restaurant> restaurants, 
            Dictionary<string, Blog> blogs)
        {
            var dto = new FavoriteDto
            {
                FavoriteId = favorite.FavoriteId,
                UserId = favorite.UserId,
                TargetType = favorite.TargetType,
                TargetId = favorite.TargetId,
                CreatedAt = favorite.CreatedAt
            };

            // Map related data based on target type
            switch (favorite.TargetType.ToUpper())
            {
                case "HOTEL":
                    if (hotels.TryGetValue(favorite.TargetId, out var hotel))
                    {
                        dto.Hotel = new HotelSummaryDto
                        {
                            HotelId = hotel.HotelId,
                            Name = hotel.Name,
                            Address = hotel.Address,
                            Description = hotel.Description,
                            Image = hotel.Image
                        };
                    }
                    break;

                case "RESTAURANT":
                    if (restaurants.TryGetValue(favorite.TargetId, out var restaurant))
                    {
                        dto.Restaurant = new RestaurantSummaryDto
                        {
                            RestaurantId = restaurant.RestaurantId,
                            Name = restaurant.Name,
                            Address = restaurant.Address,
                            Description = restaurant.Description,
                            AveragePrice = restaurant.AveragePrice,
                            Image = restaurant.Image,
                            Style = restaurant.Style
                        };
                    }
                    break;

                case "POST":
                    // Skip mapping for POST as requested - leave Post property as null
                    break;

                default:
                    // Unknown target type - leave all related data as null
                    break;
            }

            return dto;
        }
    }
}
