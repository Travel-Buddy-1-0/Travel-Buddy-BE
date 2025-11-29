using BusinessObject.DTOs;

namespace Services.Interfaces
{
    public interface IRoomService
    {
        Task<RoomDetailDto?> GetRoomDetailAsync(int roomId);
    }
}
