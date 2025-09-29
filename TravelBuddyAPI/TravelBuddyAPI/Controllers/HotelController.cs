using BusinessLogic.Services;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // -1 api suggestion hotel - trả về 4 hotel
        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] int limit = 4)
        {
            var data = await _hotelService.GetSuggestionsAsync(limit);
            return Ok(data);
        }

        // -1 api travel with top hotel- trả về 4 hotel
        [HttpGet("top")]
        public async Task<IActionResult> GetTop([FromQuery] int limit = 4)
        {
            var data = await _hotelService.GetTopHotelsAsync(limit);
            return Ok(data);
        }

        // -1 api cho search hotel với location+checkin+checkout+ Guest 
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] HotelSearchRequestDto request, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
        {
            var data = await _hotelService.SearchAsync(request, null, limit, offset);
            return Ok(data);
        }

        // -api chuyên dung cho filter theo star, kiểu , giá tiên,
        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromQuery] int limit, [FromQuery] int offset, [FromBody] HotelFilterRequestDto filter, [FromQuery] string? location, [FromQuery] int? guests)
        {
            var search = new HotelSearchRequestDto { Location = location, Guests = guests };
            var data = await _hotelService.SearchAsync(search, filter, limit, offset);
            return Ok(data);
        }

        // -1 api hotel detail nhận id trả về các trường thông tin như ảnh
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail([FromRoute] int id)
        {
            var data = await _hotelService.GetDetailAsync(id);
            return Ok(data);
        }

        // -1 api nhận vào thông tin khách hàng + ... + hotel id được đặt
        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] HotelBookingRequestDto request, [FromQuery] int userId)
        {
            var bookingId = await _hotelService.BookAsync(request, userId);
            return Ok(new { bookingId });
        }
    }
}


