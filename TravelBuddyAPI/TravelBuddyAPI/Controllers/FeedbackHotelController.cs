using BusinessLogic.Services;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeedbackHotelController : ControllerBase
    {
        private readonly IFeedbackHotelService _feedbackHotelService;

        public FeedbackHotelController(IFeedbackHotelService feedbackHotelService)
        {
            _feedbackHotelService = feedbackHotelService;
        }

        /// <summary>
        /// Get all hotel feedbacks
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllFeedbackHotels()
        {
            var data = await _feedbackHotelService.GetAllFeedbackHotelsAsync();
            return Ok(data);
        }

        /// <summary>
        /// Get hotel feedback by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackHotelById([FromRoute] int id)
        {
            var data = await _feedbackHotelService.GetFeedbackHotelByIdAsync(id);
            if (data == null)
            {
                return NotFound($"FeedbackHotel with ID {id} not found.");
            }
            return Ok(data);
        }

        /// <summary>
        /// Get hotel feedbacks by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetFeedbackHotelsByUserId([FromRoute] int userId)
        {
            var data = await _feedbackHotelService.GetFeedbackHotelsByUserIdAsync(userId);
            return Ok(data);
        }

        /// <summary>
        /// Get hotel feedbacks by hotel ID
        /// </summary>
        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetFeedbackHotelsByHotelId([FromRoute] int hotelId)
        {
            var data = await _feedbackHotelService.GetFeedbackHotelsByHotelIdAsync(hotelId);
            return Ok(data);
        }

        /// <summary>
        /// Get hotel feedbacks with filters
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> GetFeedbackHotelsByFilter([FromBody] FeedbackHotelFilterRequestDto filter)
        {
            var data = await _feedbackHotelService.GetFeedbackHotelsByFilterAsync(filter);
            return Ok(data);
        }

        /// <summary>
        /// Create new hotel feedback
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateFeedbackHotel([FromBody] FeedbackHotelCreateRequestDto request)
        {
            try
            {
                var data = await _feedbackHotelService.CreateFeedbackHotelAsync(request);
                return CreatedAtAction(nameof(GetFeedbackHotelById), new { id = data.FeedbackId }, data);
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
        /// Update hotel feedback
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedbackHotel([FromRoute] int id, [FromBody] FeedbackHotelUpdateRequestDto request)
        {
            try
            {
                var data = await _feedbackHotelService.UpdateFeedbackHotelAsync(id, request);
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
        /// Delete hotel feedback
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedbackHotel([FromRoute] int id)
        {
            try
            {
                await _feedbackHotelService.DeleteFeedbackHotelAsync(id);
                return NoContent();
            }
            catch (BusinessLogic.Exceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Check if user can create feedback for hotel
        /// </summary>
        [HttpGet("can-create")]
        public async Task<IActionResult> CanUserCreateFeedback([FromQuery] int userId, [FromQuery] int hotelId)
        {
            var canCreate = await _feedbackHotelService.CanUserCreateFeedbackAsync(userId, hotelId);
            return Ok(new { canCreate });
        }
    }
}
