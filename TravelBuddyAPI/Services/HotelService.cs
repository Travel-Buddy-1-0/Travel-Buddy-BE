using BusinessLogic.Exceptions;
using BusinessObject.DTOs;
using BusinessObject.Models;
using Repositories;

namespace BusinessLogic.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;

    public HotelService(IHotelRepository hotelRepository)
    {
        _hotelRepository = hotelRepository;
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
        return result.Take(limit)
        .OrderByDescending(h => h.AverageRating)
        .ToList();
    }

    public async Task<List<HotelSummaryDto>> SearchAsync(HotelSearchRequestDto request, HotelFilterRequestDto? filter = null, int limit = 20, int offset = 0)
    {
        var hotels = await _hotelRepository.SearchHotelsAsync(request.Location, request.Guests, filter?.MinPrice, filter?.MaxPrice, filter?.Type, limit, offset);
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
                Image = r.Image?.ToString()
            }).ToList()
        };
    }

    public async Task<int> BookAsync(HotelBookingRequestDto request, int userId)
    {
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId) ?? throw new NotFoundException($"Hotel {request.HotelId} not found");

        var detail = new BookingDetail
        {
            UserId = userId,
            HotelId = request.HotelId,
            CheckInDate = request.CheckIn,
            CheckOutDate = request.CheckOut,
            TotalPrice = null
        };
        var created = await _hotelRepository.CreateBookingAsync(detail);
        return created.BookingId;
    }

    public async Task<List<BookingHistoryDto>> GetBookingHistoryAsync(int userId, DateOnly? bookingDate)
    {
        var bookings = await _hotelRepository.GetBookingHistoryAsync(userId, bookingDate);
        return bookings.Select(b => new BookingHistoryDto
        {
            BookingId = b.BookingId,
            UserId = b.UserId,
            HotelId = b.HotelId,
            BookingDate = b.BookingDate,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            TotalPrice = b.TotalPrice,
            Approved = b.Approved
        }).ToList();
    }

    private async Task<List<HotelSummaryDto>> MapToSummaryAsync(List<Hotel> hotels)
    {
        var result = new List<HotelSummaryDto>(hotels.Count);
        foreach (var h in hotels)
        {
            var avg = await _hotelRepository.GetAverageRatingAsync(h.HotelId);
            result.Add(new HotelSummaryDto
            {
                HotelId = h.HotelId,
                Name = h.Name,
                Address = h.Address,
                Image = h.Image?.ToString(),
                AverageRating = avg
            });
        }
        return  result;
    }
}


