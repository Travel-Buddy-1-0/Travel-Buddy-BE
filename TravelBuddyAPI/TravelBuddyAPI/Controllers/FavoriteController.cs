using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        /// <summary>
        /// Get all favorites with optional filtering and pagination
        /// </summary>
        /// <param name="filter">Filter and pagination parameters</param>
        /// <returns>List of favorites</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetFavorites([FromQuery] FavoriteFilterRequestDto filter)
        {
            try
            {
                var favorites = await _favoriteService.GetFilteredAsync(filter);
                var totalCount = await _favoriteService.GetTotalCountAsync(filter);

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("X-Page-Number", filter.PageNumber.ToString());
                Response.Headers.Add("X-Page-Size", filter.PageSize.ToString());

                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving favorites", error = ex.Message });
            }
        }

        /// <summary>
        /// Get favorite by ID
        /// </summary>
        /// <param name="id">Favorite ID</param>
        /// <returns>Favorite details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<FavoriteDto>> GetFavorite(int id)
        {
            try
            {
                var favorite = await _favoriteService.GetByIdAsync(id);
                
                if (favorite == null)
                {
                    return NotFound(new { message = $"Favorite with ID {id} not found" });
                }

                return Ok(favorite);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the favorite", error = ex.Message });
            }
        }

        /// <summary>
        /// Get favorites by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user's favorites</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetFavoritesByUser(int userId)
        {
            try
            {
                var favorites = await _favoriteService.GetByUserIdAsync(userId);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user favorites", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if an item is favorited by a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="targetType">Target type (HOTEL, RESTAURANT, POST)</param>
        /// <param name="targetId">Target ID</param>
        /// <returns>Boolean indicating if item is favorited</returns>
        [HttpGet("check/{userId}/{targetType}/{targetId}")]
        public async Task<ActionResult<bool>> IsFavorite(int userId, string targetType, string targetId)
        {
            try
            {
                var isFavorite = await _favoriteService.IsFavoriteAsync(userId, targetType, targetId);
                return Ok(new { isFavorite });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking favorite status", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new favorite
        /// </summary>
        /// <param name="request">Favorite creation request</param>
        /// <returns>Created favorite</returns>
        [HttpPost]
        public async Task<ActionResult<FavoriteDto>> CreateFavorite([FromBody] FavoriteCreateRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var favorite = await _favoriteService.CreateAsync(request);
                return CreatedAtAction(nameof(GetFavorite), new { id = favorite.FavoriteId }, favorite);
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the favorite", error = ex.Message });
            }
        }

        /// <summary>
        /// Toggle favorite status (add if not exists, remove if exists)
        /// </summary>
        /// <param name="request">Favorite toggle request</param>
        /// <returns>Toggle result</returns>
        [HttpPost("toggle")]
        public async Task<ActionResult> ToggleFavorite([FromBody] FavoriteCreateRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _favoriteService.ToggleFavoriteAsync(request);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while toggling the favorite", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing favorite
        /// </summary>
        /// <param name="id">Favorite ID</param>
        /// <param name="request">Favorite update request</param>
        /// <returns>Updated favorite</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<FavoriteDto>> UpdateFavorite(int id, [FromBody] FavoriteCreateRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var favorite = await _favoriteService.UpdateAsync(id, request);
                
                if (favorite == null)
                {
                    return NotFound(new { message = $"Favorite with ID {id} not found" });
                }

                return Ok(favorite);
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the favorite", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a favorite
        /// </summary>
        /// <param name="id">Favorite ID</param>
        /// <returns>Delete result</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFavorite(int id)
        {
            try
            {
                var result = await _favoriteService.DeleteAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = $"Favorite with ID {id} not found" });
                }

                return Ok(new { message = "Favorite deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the favorite", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove favorite by user and target
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="targetType">Target type</param>
        /// <param name="targetId">Target ID</param>
        /// <returns>Delete result</returns>
        [HttpDelete("user/{userId}/{targetType}/{targetId}")]
        public async Task<ActionResult> RemoveFavoriteByTarget(int userId, string targetType, string targetId)
        {
            try
            {
                var request = new FavoriteCreateRequestDto
                {
                    UserId = userId,
                    TargetType = targetType,
                    TargetId = targetId
                };

                var result = await _favoriteService.ToggleFavoriteAsync(request);
                
                if (!result)
                {
                    return NotFound(new { message = $"Favorite not found for user {userId} and target {targetType}:{targetId}" });
                }

                return Ok(new { message = "Favorite removed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing the favorite", error = ex.Message });
            }
        }
    }
}
