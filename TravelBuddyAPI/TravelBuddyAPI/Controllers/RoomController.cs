using BusinessLogic.Services;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services;


namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        /// <summary>
        /// Get room detail by room ID
        /// </summary>
        /// <param name="id">Room ID</param>
        /// <returns>Room detail information</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDetailDto>> GetRoomDetail(int id)
        {
            try
            {
                var roomDetail = await _roomService.GetRoomDetailAsync(id);
                
                if (roomDetail == null)
                {
                    return NotFound(new { message = $"Room with ID {id} not found" });
                }

                return Ok(roomDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving room details", error = ex.Message });
            }
        }
    }
}
