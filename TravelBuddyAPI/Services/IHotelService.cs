using BusinessObject.DTOs;

namespace BusinessLogic.Services;

public interface IHotelService
{
    Task<List<HotelSummaryDto>> GetSuggestionsAsync(int limit = 4);
    Task<List<HotelSummaryDto>> GetTopHotelsAsync(int limit = 4);
    Task<List<HotelSummaryDto>> SearchAsync(HotelSearchRequestDto request, int limit = 20, int offset = 0);
    Task<HotelDetailDto> GetDetailAsync(int hotelId);
    Task<int> BookAsync(HotelBookingRequestDto request, int userId);
    Task<List<BookingHistoryDto>> GetBookingHistoryAsync(int userId, DateOnly? bookingDate);
    Task<List<ReviewDto>> GetReviewsAsync(int hotelId, int? rating, int limit = 20, int offset = 0);
    Task<int> ChangeStatusBookingAsync(int bookingId, int status);

    Task<List<VoucherDto>> GetActiveVouchersAsync();

    Task<VoucherDto?> GetVoucherByCodeAsync(string code);
}


