using BusinessLogic.Exceptions;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using Repositories;

namespace BusinessLogic.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IUserRepository _userRepository;

    public HotelService(IHotelRepository hotelRepository, IUserRepository userRepository)
    {
        _hotelRepository = hotelRepository;
        _userRepository = userRepository;
    }

    public async Task<List<HotelSummaryDto>> GetSuggestionsAsync(int limit = 4)
    {
        var hotels = await _hotelRepository.GetSuggestionsAsync(limit);
        return await MapToSummaryAsync(hotels);
    }

    public async Task<List<HotelSummaryDto>> GetTopHotelsAsync(int limit)
    {
        var hotels = await _hotelRepository.GetTopHotelsAsync(25);
        var result = await MapToSummaryAsync(hotels);
        result = result.OrderByDescending(h => h.AverageRating)
        .ToList();
        return result.Take(limit).ToList();
        ;
    }
    public static string CleanProvinceName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;

        name = name.Trim().ToLower();

        if (name.StartsWith("tỉnh "))
            return name.Substring("tỉnh ".Length).Trim();

        if (name.StartsWith("thành phố "))
            return name.Substring("thành phố ".Length).Trim();

        return name;
    }

    public async Task<List<HotelSummaryDto>> SearchAsync(HotelSearchRequestDto request, int limit = 20, int offset = 0)
    {
        var hotels = await _hotelRepository.SearchHotelsAsync(CleanProvinceName(request.Location), request.Guests, null, null, request?.Type, request?.Stars, request?.Amenities, limit, offset);
        return await MapToSummaryAsync(hotels);
    }

    public async Task<HotelDetailDto> GetDetailAsync(int hotelId)
    {
        var hotel = await _hotelRepository.GetByIdAsync(hotelId) ?? throw new NotFoundException($"Hotel {hotelId} not found");
        var rooms = await _hotelRepository.GetRoomsByHotelAsync(hotelId);
        var rating = await _hotelRepository.GetAverageRatingAsync(hotelId);
        return new HotelDetailDto
        {
            HotelId = hotel.HotelId,
            Name = hotel.Name,
            Description = hotel.Description,
            Address = hotel.Address,
            Image = hotel.Image?.ToString(),
            Style = hotel.Style?.ToString(),
            AverageRating = rating,
            Rooms = rooms.Select(r => new HotelRoomDto
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                RoomType = r.RoomType,
                PricePerNight = r.PricePerNight,
                Capacity = r.Capacity,
                IsAvailable = r.IsAvailable,
                Image = r.Image?.ToString(),
                LstCheckoutDate = r.Bookingdetails.OrderByDescending(x => x.CheckOutDate).FirstOrDefault()?.CheckOutDate
            }).ToList()
        };
    }

    public async Task<int> BookAsync(HotelBookingRequestDto request, int userId)
    {
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId) ?? throw new NotFoundException($"Hotel {request.HotelId} not found");
        if (request.TypePayment == 2)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {

                throw new NotFoundException($"User {request.HotelId} not found");
            }
            else
            {
                if (request.TotalPrice > user.WalletBalance)
                {
                    throw new Exception($"Your wallet balance is insufficient!");
                }
            }
        }

        var detail = new Bookingdetail
        {
            UserId = userId,
            HotelId = request.HotelId,
            CheckInDate = request.CheckIn,
            CheckOutDate = request.CheckOut,
            TotalPrice = request.TotalPrice,
            Status = 1,
            RoomId = request.RoomId,
            RestaurantId = request.RestaurantId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Note = request.Note,
            Country = request.Country
        };
        if (request.TypePayment == 0)
        {
            detail.Status = 0;
        }
        else if (request.TypePayment == 1 || request.TypePayment == 2)
        {
            detail.Status = 1;
        }
        var created = await _hotelRepository.CreateBookingAsync(detail);
        return created.BookingId;
    }

    public async Task<List<BookingHistoryDto>> GetBookingHistoryAsync(int userId, DateOnly? bookingDate)
    {
        var bookings = await _hotelRepository.GetBookingHistoryAsync(userId, bookingDate);
        return bookings.Select(b => new BookingHistoryDto
        {
            BookingId = b.BookingId,
            UserId = b.UserId ?? 0,
            HotelId = b.HotelId,
            RoomId= b.RoomId,
            BookingDate = b.BookingDate ?? DateTime.UtcNow,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            TotalPrice = b.TotalPrice,
            Approved = b.Approved ?? false,
            FirstName = b.FirstName,
            LastName = b.LastName,
            Email = b.Email,
            Phone = b.Phone,
            Note = b.Note,
            Country = b.Country,
            Status = b.Status

        }).ToList();
    }

    public async Task<List<ReviewDto1>> GetReviewsAsync(int hotelId, int? rating, int limit = 20, int offset = 0)
    {
        var reviews = await _hotelRepository.GetReviewsByHotelAsync(hotelId, rating, limit, offset);
        return reviews.Select(r => new ReviewDto1
        {
            ReviewId = r.ReviewId,
            UserId = r.UserId,
            ReviewerName = r.User?.FullName ?? r.User?.Username,
            HotelId = r.HotelId,
            Rating = r.Rating,
            Comment = r.Comment,
            Image = r.Image,
            ReviewDate = r.ReviewDate
        }).ToList();
    }

    private async Task<List<HotelSummaryDto>> MapToSummaryAsync(List<Hotel> hotels)
    {
        var result = new List<HotelSummaryDto>(hotels.Count);

        foreach (var h in hotels)
        {
            var avg = await _hotelRepository.GetAverageRatingAsync(h.HotelId);
            var rooms = await _hotelRepository.GetRoomsByHotelAsync(h.HotelId);

            result.Add(new HotelSummaryDto
            {
                HotelId = h.HotelId,
                Name = h.Name,
                Address = h.Address,
                Image = h.Image?.ToString(),
                AverageRating = avg,
                Style = h.Style,
                Description = h.Description,
                Rooms = rooms.Select(r => new HotelRoomDto
                {
                    RoomId = r.RoomId,

                    PricePerNight = r.PricePerNight,


                }).ToList()
            });
        }

        return result;
    }

    public Task<int> ChangeStatusBookingAsync(int bookingId, int status)
    {
        return _hotelRepository.ChangeStatusBookingAsync(bookingId, status);
    }
}


