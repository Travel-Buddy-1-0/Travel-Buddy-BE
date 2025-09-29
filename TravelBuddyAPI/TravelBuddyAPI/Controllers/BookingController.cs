using BusinessObject.DTOs;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Create a new hotel booking
        /// </summary>
        /// <param name="bookingRequest">Hotel booking request details</param>
        /// <returns>Created booking information</returns>
        [HttpPost("hotel")]
        public async Task<IActionResult> CreateHotelBooking([FromBody] HotelBookingRequestDto bookingRequest)
        {
            try
            {
                if (bookingRequest == null)
                {
                    return BadRequest("Booking request cannot be null.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var booking = await _bookingService.CreateHotelBookingAsync(bookingRequest);
                return CreatedAtAction(nameof(GetBookingById), new { id = booking.BookingId }, booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the booking.");
            }
        }

        /// <summary>
        /// Get booking by ID
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                {
                    return NotFound($"Booking with ID {id} not found.");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the booking.");
            }
        }

        /// <summary>
        /// Get booking by confirmation number
        /// </summary>
        /// <param name="confirmationNumber">Confirmation number</param>
        /// <returns>Booking details</returns>
        [HttpGet("confirmation/{confirmationNumber}")]
        public async Task<IActionResult> GetBookingByConfirmationNumber(string confirmationNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(confirmationNumber))
                {
                    return BadRequest("Confirmation number cannot be null or empty.");
                }

                var booking = await _bookingService.GetBookingByConfirmationNumberAsync(confirmationNumber);
                if (booking == null)
                {
                    return NotFound($"Booking with confirmation number {confirmationNumber} not found.");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the booking.");
            }
        }

        /// <summary>
        /// Get all bookings for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user's bookings</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBookingsByUserId(int userId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching user bookings.");
            }
        }

        /// <summary>
        /// Get all bookings for a specific hotel
        /// </summary>
        /// <param name="hotelId">Hotel ID</param>
        /// <returns>List of hotel's bookings</returns>
        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetBookingsByHotelId(int hotelId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByHotelIdAsync(hotelId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching hotel bookings.");
            }
        }

        /// <summary>
        /// Update an existing booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="bookingRequest">Updated booking details</param>
        /// <returns>Updated booking information</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] HotelBookingRequestDto bookingRequest)
        {
            try
            {
                if (bookingRequest == null)
                {
                    return BadRequest("Booking request cannot be null.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var booking = await _bookingService.UpdateBookingAsync(id, bookingRequest);
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the booking.");
            }
        }

        /// <summary>
        /// Delete a booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var result = await _bookingService.DeleteBookingAsync(id);
                if (!result)
                {
                    return NotFound($"Booking with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the booking.");
            }
        }

        /// <summary>
        /// Approve a booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> ApproveBooking(int id)
        {
            try
            {
                var result = await _bookingService.ApproveBookingAsync(id);
                if (!result)
                {
                    return NotFound($"Booking with ID {id} not found.");
                }

                return Ok(new { message = "Booking approved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while approving the booking.");
            }
        }

        /// <summary>
        /// Cancel a booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(id);
                if (!result)
                {
                    return NotFound($"Booking with ID {id} not found.");
                }

                return Ok(new { message = "Booking cancelled successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while cancelling the booking.");
            }
        }

        /// <summary>
        /// Get bookings within a date range
        /// </summary>
        /// <param name="startDate">Start date (yyyy-MM-dd)</param>
        /// <param name="endDate">End date (yyyy-MM-dd)</param>
        /// <returns>List of bookings in date range</returns>
        [HttpGet("date-range")]
        public async Task<IActionResult> GetBookingsByDateRange([FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                {
                    return BadRequest("Start date and end date are required.");
                }

                if (!DateOnly.TryParse(startDate, out var start) || !DateOnly.TryParse(endDate, out var end))
                {
                    return BadRequest("Invalid date format. Use yyyy-MM-dd format.");
                }

                if (start > end)
                {
                    return BadRequest("Start date must be before or equal to end date.");
                }

                var bookings = await _bookingService.GetBookingsByDateRangeAsync(start, end);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching bookings by date range.");
            }
        }

        /// <summary>
        /// Get booking confirmation details
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking confirmation information</returns>
        [HttpGet("{id}/confirmation")]
        public async Task<IActionResult> GetBookingConfirmation(int id)
        {
            try
            {
                var confirmation = await _bookingService.GetBookingConfirmationAsync(id);
                return Ok(confirmation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching booking confirmation.");
            }
        }

        /// <summary>
        /// Check hotel availability
        /// </summary>
        /// <param name="hotelId">Hotel ID</param>
        /// <param name="checkInDate">Check-in date (yyyy-MM-dd)</param>
        /// <param name="checkOutDate">Check-out date (yyyy-MM-dd)</param>
        /// <param name="numberOfRooms">Number of rooms needed</param>
        /// <returns>Availability status</returns>
        [HttpGet("availability")]
        public async Task<IActionResult> CheckHotelAvailability([FromQuery] int hotelId, [FromQuery] string checkInDate, [FromQuery] string checkOutDate, [FromQuery] int numberOfRooms)
        {
            try
            {
                if (string.IsNullOrEmpty(checkInDate) || string.IsNullOrEmpty(checkOutDate))
                {
                    return BadRequest("Check-in date and check-out date are required.");
                }

                if (!DateOnly.TryParse(checkInDate, out var checkIn) || !DateOnly.TryParse(checkOutDate, out var checkOut))
                {
                    return BadRequest("Invalid date format. Use yyyy-MM-dd format.");
                }

                if (checkIn >= checkOut)
                {
                    return BadRequest("Check-in date must be before check-out date.");
                }

                if (numberOfRooms <= 0)
                {
                    return BadRequest("Number of rooms must be greater than 0.");
                }

                var isAvailable = await _bookingService.IsHotelAvailableAsync(hotelId, checkIn, checkOut, numberOfRooms);
                return Ok(new { 
                    hotelId = hotelId,
                    checkInDate = checkIn,
                    checkOutDate = checkOut,
                    numberOfRooms = numberOfRooms,
                    isAvailable = isAvailable
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while checking hotel availability.");
            }
        }
    }
}
