using BusinessLogic.Exceptions;
using BusinessObject.DTOs;
using BusinessObject.Models;
using Repositories;

namespace BusinessLogic.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<List<HotelResponseDto>> GetSuggestedHotelsAsync()
        {
            var hotels = await _hotelRepository.GetSuggestedHotelsAsync(4);
            return hotels.Select(MapToHotelResponseDto).ToList();
        }

        public async Task<List<HotelResponseDto>> GetTopHotelsAsync()
        {
            var hotels = await _hotelRepository.GetTopHotelsAsync(4);
            return hotels.Select(MapToHotelResponseDto).ToList();
        }

        public async Task<List<HotelResponseDto>> SearchHotelsAsync(HotelSearchRequestDto searchRequest)
        {
            var hotels = await _hotelRepository.SearchHotelsAsync(
                searchRequest.Location,
                searchRequest.CheckIn,
                searchRequest.CheckOut,
                searchRequest.Guests
            );

            return hotels.Select(MapToHotelResponseDto).ToList();
        }

        public async Task<List<HotelResponseDto>> FilterHotelsAsync(HotelFilterRequestDto filterRequest)
        {
            var hotels = await _hotelRepository.FilterHotelsAsync(
                filterRequest.Style,
                filterRequest.Location
            );

            // Apply pagination
            var skip = (filterRequest.Page - 1) * filterRequest.PageSize;
            var pagedHotels = hotels.Skip(skip).Take(filterRequest.PageSize).ToList();

            return pagedHotels.Select(MapToHotelResponseDto).ToList();
        }

        public async Task<HotelResponseDto> GetHotelDetailAsync(int hotelId)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(hotelId);
            if (hotel == null)
            {
                throw new NotFoundException($"Hotel with ID {hotelId} not found.");
            }

            var rooms = await _hotelRepository.GetHotelRoomsAsync(hotelId);
            var hotelDto = MapToHotelResponseDto(hotel);
            hotelDto.Rooms = rooms.Select(MapToRoomResponseDto).ToList();

            return hotelDto;
        }

        public async Task<List<Hotel>> GetAllHotelsAsync()
        {
            return await _hotelRepository.GetAllHotelsAsync();
        }

        public async Task<Hotel> CreateHotelAsync(Hotel hotel)
        {
            var existingHotel = await _hotelRepository.GetHotelByIdAsync(hotel.HotelId);
            if (existingHotel != null)
            {
                throw new ConflictException("Hotel with this ID already exists.");
            }

            return await _hotelRepository.AddHotelAsync(hotel);
        }

        public async Task<Hotel> UpdateHotelAsync(Hotel hotel)
        {
            var existingHotel = await _hotelRepository.GetHotelByIdAsync(hotel.HotelId);
            if (existingHotel == null)
            {
                throw new NotFoundException($"Hotel with ID {hotel.HotelId} not found.");
            }

            return await _hotelRepository.UpdateHotelAsync(hotel);
        }

        public async Task DeleteHotelAsync(int hotelId)
        {
            var existingHotel = await _hotelRepository.GetHotelByIdAsync(hotelId);
            if (existingHotel == null)
            {
                throw new NotFoundException($"Hotel with ID {hotelId} not found.");
            }

            await _hotelRepository.DeleteHotelAsync(hotelId);
        }

        private HotelResponseDto MapToHotelResponseDto(Hotel hotel)
        {
            return new HotelResponseDto
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Description = hotel.Description,
                Address = hotel.Address,
                Image = hotel.Image,
                Style = hotel.Style
            };
        }

        private RoomResponseDto MapToRoomResponseDto(Room room)
        {
            return new RoomResponseDto
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                IsAvailable = room.IsAvailable ?? false,
                Image = room.Image
            };
        }
    }
}
