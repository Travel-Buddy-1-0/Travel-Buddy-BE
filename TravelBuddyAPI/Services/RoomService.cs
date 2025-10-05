using BusinessObject.DTOs;
using Repositories;

namespace Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<RoomDetailDto?> GetRoomDetailAsync(int roomId)
        {
            var room = await _roomRepository.GetByIdWithHotelAsync(roomId);
            
            if (room == null)
            {
                return null;
            }

            // Lấy thông tin last checkout date
            var lastCheckoutDate = await _roomRepository.GetLastCheckoutDateAsync(roomId);

            return new RoomDetailDto
            {
                RoomId = room.RoomId,
                HotelId = room.HotelId,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                IsAvailable = room.IsAvailable,
                Image = room.Image,
                LastCheckoutDate = lastCheckoutDate,
                Hotel = room.Hotel != null ? new HotelSummaryDto
                {
                    HotelId = room.Hotel.HotelId,
                    Name = room.Hotel.Name,
                    Address = room.Hotel.Address,
                    Description = room.Hotel.Description,
                    Image = room.Hotel.Image
                } : null
            };
        }
    }
}
