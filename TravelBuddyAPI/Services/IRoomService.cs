using BusinessObject.DTOs;

namespace Services
{
    public interface IRoomService
    {
        Task<RoomDetailDto?> GetRoomDetailAsync(int roomId);
    }
}
