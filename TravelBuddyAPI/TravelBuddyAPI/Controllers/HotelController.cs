using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        /// <summary>
        /// Get suggested hotels (4 hotels)
        /// </summary>
        [HttpGet("suggested")]
        public async Task<IActionResult> GetSuggestedHotels()
        {
            try
            {
                var hotels = await _hotelService.GetSuggestedHotelsAsync();
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching suggested hotels.");
            }
        }

        /// <summary>
        /// Get top hotels for travel (4 hotels)
        /// </summary>
        [HttpGet("top")]
        public async Task<IActionResult> GetTopHotels()
        {
            try
            {
                var hotels = await _hotelService.GetTopHotelsAsync();
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching top hotels.");
            }
        }

        /// <summary>
        /// Search hotels by location, check-in, check-out, and guests
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> SearchHotels([FromBody] HotelSearchRequestDto searchRequest)
        {
            try
            {
                if (searchRequest == null)
                {
                    return BadRequest("Search request cannot be null.");
                }

                var hotels = await _hotelService.SearchHotelsAsync(searchRequest);
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while searching hotels.");
            }
        }

        /// <summary>
        /// Filter hotels by star rating, style, price range, etc.
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> FilterHotels([FromBody] HotelFilterRequestDto filterRequest)
        {
            try
            {
                if (filterRequest == null)
                {
                    return BadRequest("Filter request cannot be null.");
                }

                var hotels = await _hotelService.FilterHotelsAsync(filterRequest);
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while filtering hotels.");
            }
        }

        /// <summary>
        /// Get hotel detail by ID including images and rooms
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelDetail(int id)
        {
            try
            {
                var hotel = await _hotelService.GetHotelDetailAsync(id);
                return Ok(hotel);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching hotel details.");
            }
        }

        /// <summary>
        /// Get all hotels
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllHotels()
        {
            try
            {
                var hotels = await _hotelService.GetAllHotelsAsync();
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching hotels.");
            }
        }

        /// <summary>
        /// Create a new hotel
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateHotel([FromBody] BusinessObject.Models.Hotel hotel)
        {
            try
            {
                if (hotel == null)
                {
                    return BadRequest("Hotel data cannot be null.");
                }

                var createdHotel = await _hotelService.CreateHotelAsync(hotel);
                return CreatedAtAction(nameof(GetHotelDetail), new { id = createdHotel.HotelId }, createdHotel);
            }
            catch (ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the hotel.");
            }
        }

        /// <summary>
        /// Update hotel information
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] BusinessObject.Models.Hotel hotel)
        {
            try
            {
                if (hotel == null)
                {
                    return BadRequest("Hotel data cannot be null.");
                }

                if (id != hotel.HotelId)
                {
                    return BadRequest("Hotel ID mismatch.");
                }

                var updatedHotel = await _hotelService.UpdateHotelAsync(hotel);
                return Ok(updatedHotel);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the hotel.");
            }
        }

        /// <summary>
        /// Delete a hotel
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                await _hotelService.DeleteHotelAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the hotel.");
            }
        }
    }
}
