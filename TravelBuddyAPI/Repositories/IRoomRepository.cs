using BusinessObject.Entities;

namespace Repositories
{
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(int roomId);
        Task<Room?> GetByIdWithHotelAsync(int roomId);
        Task<DateOnly?> GetLastCheckoutDateAsync(int roomId);
    }
}
