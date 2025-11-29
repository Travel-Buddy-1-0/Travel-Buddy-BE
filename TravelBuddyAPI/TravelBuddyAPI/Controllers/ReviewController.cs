using BusinessLogic.Services;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Get all reviews
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var data = await _reviewService.GetAllReviewsAsync();
            return Ok(data);
        }

        /// <summary>
        /// Get review by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById([FromRoute] int id)
        {
            var data = await _reviewService.GetReviewByIdAsync(id);
            if (data == null)
            {
                return NotFound($"Review with ID {id} not found.");
            }
            return Ok(data);
        }

        /// <summary>
        /// Get reviews by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReviewsByUserId([FromRoute] int userId)
        {
            var data = await _reviewService.GetReviewsByUserIdAsync(userId);
            return Ok(data);
        }

        /// <summary>
        /// Get reviews by hotel ID
        /// </summary>
        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetReviewsByHotelId([FromRoute] int hotelId)
        {
            var data = await _reviewService.GetReviewsByHotelIdAsync(hotelId);
            return Ok(data);
        }

        /// <summary>
        /// Get reviews by restaurant ID
        /// </summary>
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetReviewsByRestaurantId([FromRoute] int restaurantId)
        {
            var data = await _reviewService.GetReviewsByRestaurantIdAsync(restaurantId);
            return Ok(data);
        }

        /// <summary>
        /// Get reviews by tour ID
        /// </summary>
        [HttpGet("tour/{tourId}")]
        public async Task<IActionResult> GetReviewsByTourId([FromRoute] int tourId)
        {
            var data = await _reviewService.GetReviewsByTourIdAsync(tourId);
            return Ok(data);
        }

        /// <summary>
        /// Get reviews with filters
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> GetReviewsByFilter([FromBody] ReviewFilterRequestDto filter)
        {
            var data = await _reviewService.GetReviewsByFilterAsync(filter);
            return Ok(data);
        }

        /// <summary>
        /// Create new review
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateRequestDto request)
        {
            try
            {
                var data = await _reviewService.CreateReviewAsync(request);
                return CreatedAtAction(nameof(GetReviewById), new { id = data.ReviewId }, data);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (BusinessLogic.Exceptions.ConflictException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Update review
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview([FromRoute] int id, [FromBody] ReviewUpdateRequestDto request)
        {
            try
            {
                var data = await _reviewService.UpdateReviewAsync(id, request);
                return Ok(data);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (BusinessLogic.Exceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Delete review
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview([FromRoute] int id)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(id);
                return NoContent();
            }
            catch (BusinessLogic.Exceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Check if user can create review for entity
        /// </summary>
        [HttpGet("can-create")]
        public async Task<IActionResult> CanUserCreateReview(
            [FromQuery] int userId, 
            [FromQuery] int? tourId = null, 
            [FromQuery] int? hotelId = null, 
            [FromQuery] int? restaurantId = null)
        {
            var canCreate = await _reviewService.CanUserCreateReviewAsync(userId, tourId, hotelId, restaurantId);
            return Ok(new { canCreate });
        }
    }
}
