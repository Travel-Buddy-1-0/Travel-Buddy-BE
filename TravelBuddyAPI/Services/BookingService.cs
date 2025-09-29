using BusinessObject.DTOs;
using BusinessObject.Models;
using Repositories;
using System.Text.Json;

namespace Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IUserRepository _userRepository;

        public BookingService(IBookingRepository bookingRepository, IHotelRepository hotelRepository, IUserRepository userRepository)
        {
            _bookingRepository = bookingRepository;
            _hotelRepository = hotelRepository;
            _userRepository = userRepository;
        }

        public async Task<HotelBookingResponseDto> CreateHotelBookingAsync(HotelBookingRequestDto bookingRequest)
        {
            try
            {
                // Validate hotel exists
                var hotel = await _hotelRepository.GetHotelByIdAsync(bookingRequest.HotelId);
                if (hotel == null)
                {
                    throw new Exception($"Hotel with ID {bookingRequest.HotelId} not found.");
                }

                // Validate user exists
                var user = await _userRepository.GetUserByIdAsync(bookingRequest.UserId);
                if (user == null)
                {
                    throw new Exception($"User with ID {bookingRequest.UserId} not found.");
                }

                // Check hotel availability
                var isAvailable = await _bookingRepository.IsHotelAvailableAsync(
                    bookingRequest.HotelId, 
                    bookingRequest.CheckInDate, 
                    bookingRequest.CheckOutDate, 
                    bookingRequest.NumberOfRooms);

                if (!isAvailable)
                {
                    throw new Exception("Hotel is not available for the selected dates and number of rooms.");
                }

                // Validate dates
                if (bookingRequest.CheckInDate >= bookingRequest.CheckOutDate)
                {
                    throw new Exception("Check-in date must be before check-out date.");
                }

                if (bookingRequest.CheckInDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    throw new Exception("Check-in date cannot be in the past.");
                }

                // Calculate total price (simplified - in real app, this would be more complex)
                var pricePerNight = 100m; // This should come from hotel configuration
                var totalPrice = pricePerNight * bookingRequest.NumberOfNights * bookingRequest.NumberOfRooms;

                // Create booking detail
                var bookingDetail = new BookingDetail
                {
                    UserId = bookingRequest.UserId,
                    HotelId = bookingRequest.HotelId,
                    CheckInDate = bookingRequest.CheckInDate,
                    CheckOutDate = bookingRequest.CheckOutDate,
                    NumberOfNights = bookingRequest.NumberOfNights,
                    NumberOfGuests = bookingRequest.NumberOfGuests,
                    NumberOfRooms = bookingRequest.NumberOfRooms,
                    GuestArrivalTime = bookingRequest.GuestArrivalTime,
                    SpecialRequirements = bookingRequest.SpecialRequirements,
                    GuestInformation = (System.Text.Json.Nodes.JsonObject)JsonSerializer.SerializeToNode(bookingRequest.GuestInformation),
                    TotalPrice = totalPrice,
                    Approved = false,
                    BookingStatus = "Pending",
                    BookingDate = DateTime.UtcNow
                };

                // Create booking
                var createdBooking = await _bookingRepository.CreateBookingAsync(bookingDetail);

                // Return response
                return await MapToBookingResponseDto(createdBooking);
            }
            catch (Exception ex) when (!(ex is Exception || ex is Exception))
            {
                throw new Exception($"Error creating hotel booking: {ex.Message}", ex);
            }
        }

        public async Task<HotelBookingResponseDto?> GetBookingByIdAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    return null;
                }

                return await MapToBookingResponseDto(booking);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting booking by ID: {ex.Message}", ex);
            }
        }

        public async Task<HotelBookingResponseDto?> GetBookingByConfirmationNumberAsync(string confirmationNumber)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByConfirmationNumberAsync(confirmationNumber);
                if (booking == null)
                {
                    return null;
                }

                return await MapToBookingResponseDto(booking);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting booking by confirmation number: {ex.Message}", ex);
            }
        }

        public async Task<List<HotelBookingResponseDto>> GetBookingsByUserIdAsync(int userId)
        {
            try
            {
                var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
                var result = new List<HotelBookingResponseDto>();

                foreach (var booking in bookings)
                {
                    result.Add(await MapToBookingResponseDto(booking));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting bookings by user ID: {ex.Message}", ex);
            }
        }

        public async Task<List<HotelBookingResponseDto>> GetBookingsByHotelIdAsync(int hotelId)
        {
            try
            {
                var bookings = await _bookingRepository.GetBookingsByHotelIdAsync(hotelId);
                var result = new List<HotelBookingResponseDto>();

                foreach (var booking in bookings)
                {
                    result.Add(await MapToBookingResponseDto(booking));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting bookings by hotel ID: {ex.Message}", ex);
            }
        }

        public async Task<HotelBookingResponseDto> UpdateBookingAsync(int bookingId, HotelBookingRequestDto bookingRequest)
        {
            try
            {
                var existingBooking = await _bookingRepository.GetBookingByIdAsync(bookingId);
                if (existingBooking == null)
                {
                    throw new Exception($"Booking with ID {bookingId} not found.");
                }

                // Validate hotel exists
                var hotel = await _hotelRepository.GetHotelByIdAsync(bookingRequest.HotelId);
                if (hotel == null)
                {
                    throw new Exception($"Hotel with ID {bookingRequest.HotelId} not found.");
                }

                // Validate user exists
                var user = await _userRepository.GetUserByIdAsync(bookingRequest.UserId);
                if (user == null)
                {
                    throw new Exception($"User with ID {bookingRequest.UserId} not found.");
                }

                // Check hotel availability (excluding current booking)
                var isAvailable = await _bookingRepository.IsHotelAvailableAsync(
                    bookingRequest.HotelId, 
                    bookingRequest.CheckInDate, 
                    bookingRequest.CheckOutDate, 
                    bookingRequest.NumberOfRooms);

                if (!isAvailable)
                {
                    throw new Exception("Hotel is not available for the selected dates and number of rooms.");
                }

                // Update booking details
                existingBooking.UserId = bookingRequest.UserId;
                existingBooking.HotelId = bookingRequest.HotelId;
                existingBooking.CheckInDate = bookingRequest.CheckInDate;
                existingBooking.CheckOutDate = bookingRequest.CheckOutDate;
                existingBooking.NumberOfNights = bookingRequest.NumberOfNights;
                existingBooking.NumberOfGuests = bookingRequest.NumberOfGuests;
                existingBooking.NumberOfRooms = bookingRequest.NumberOfRooms;
                existingBooking.GuestArrivalTime = bookingRequest.GuestArrivalTime;
                existingBooking.SpecialRequirements = bookingRequest.SpecialRequirements;
                existingBooking.GuestInformation = (System.Text.Json.Nodes.JsonObject?)JsonSerializer.SerializeToNode(bookingRequest.GuestInformation);

                // Recalculate total price
                var pricePerNight = 100m;
                existingBooking.TotalPrice = pricePerNight * bookingRequest.NumberOfNights * bookingRequest.NumberOfRooms;

                var updatedBooking = await _bookingRepository.UpdateBookingAsync(existingBooking);
                return await MapToBookingResponseDto(updatedBooking);
            }
            catch (Exception ex) when (!(ex is Exception || ex is Exception))
            {
                throw new Exception($"Error updating booking: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    throw new Exception($"Booking with ID {bookingId} not found.");
                }

                return await _bookingRepository.DeleteBookingAsync(bookingId);
            }
            catch (Exception ex) when (!(ex is Exception))
            {
                throw new Exception($"Error deleting booking: {ex.Message}", ex);
            }
        }

        public async Task<bool> ApproveBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    throw new Exception($"Booking with ID {bookingId} not found.");
                }

                return await _bookingRepository.ApproveBookingAsync(bookingId);
            }
            catch (Exception ex) when (!(ex is Exception))
            {
                throw new Exception($"Error approving booking: {ex.Message}", ex);
            }
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    throw new Exception($"Booking with ID {bookingId} not found.");
                }

                return await _bookingRepository.CancelBookingAsync(bookingId);
            }
            catch (Exception ex) when (!(ex is Exception))
            {
                throw new Exception($"Error cancelling booking: {ex.Message}", ex);
            }
        }

        public async Task<List<HotelBookingResponseDto>> GetBookingsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                var bookings = await _bookingRepository.GetBookingsByDateRangeAsync(startDate, endDate);
                var result = new List<HotelBookingResponseDto>();

                foreach (var booking in bookings)
                {
                    result.Add(await MapToBookingResponseDto(booking));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting bookings by date range: {ex.Message}", ex);
            }
        }

        public async Task<BookingConfirmationDto> GetBookingConfirmationAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    throw new Exception($"Booking with ID {bookingId} not found.");
                }

                var hotel = await _hotelRepository.GetHotelByIdAsync(booking.HotelId ?? 0);
                if (hotel == null)
                {
                    throw new Exception($"Hotel not found for booking {bookingId}.");
                }

                var guestInfo = booking.GuestInformation != null 
                    ? JsonSerializer.Deserialize<GuestInformationDto>(booking.GuestInformation.ToString()!) 
                    : new GuestInformationDto();

                return new BookingConfirmationDto
                {
                    BookingId = booking.BookingId,
                    ConfirmationNumber = booking.ConfirmationNumber ?? "",
                    HotelName = hotel.Name,
                    CheckInDate = booking.CheckInDate ?? DateOnly.MinValue,
                    CheckOutDate = booking.CheckOutDate ?? DateOnly.MinValue,
                    NumberOfNights = booking.NumberOfNights ?? 0,
                    NumberOfGuests = booking.NumberOfGuests ?? 0,
                    NumberOfRooms = booking.NumberOfRooms ?? 0,
                    TotalPrice = booking.TotalPrice ?? 0,
                    GuestName = guestInfo?.FullName ?? "",
                    GuestEmail = guestInfo?.Email ?? "",
                    GuestPhone = guestInfo?.PhoneNumber ?? "",
                    BookingDate = booking.BookingDate,
                    BookingStatus = booking.BookingStatus
                };
            }
            catch (Exception ex) when (!(ex is Exception))
            {
                throw new Exception($"Error getting booking confirmation: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsHotelAvailableAsync(int hotelId, DateOnly checkInDate, DateOnly checkOutDate, int numberOfRooms)
        {
            try
            {
                return await _bookingRepository.IsHotelAvailableAsync(hotelId, checkInDate, checkOutDate, numberOfRooms);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking hotel availability: {ex.Message}", ex);
            }
        }

        private async Task<HotelBookingResponseDto> MapToBookingResponseDto(BookingDetail booking)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(booking.HotelId ?? 0);
            var guestInfo = booking.GuestInformation != null 
                ? JsonSerializer.Deserialize<GuestInformationDto>(booking.GuestInformation.ToString()!) 
                : new GuestInformationDto();

            return new HotelBookingResponseDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                HotelId = booking.HotelId ?? 0,
                HotelName = hotel?.Name ?? "Unknown Hotel",
                HotelAddress = hotel?.Address ?? "Unknown Address",
                CheckInDate = booking.CheckInDate ?? DateOnly.MinValue,
                CheckOutDate = booking.CheckOutDate ?? DateOnly.MinValue,
                NumberOfNights = booking.NumberOfNights ?? 0,
                NumberOfGuests = booking.NumberOfGuests ?? 0,
                NumberOfRooms = booking.NumberOfRooms ?? 0,
                GuestArrivalTime = booking.GuestArrivalTime ?? TimeOnly.MinValue,
                SpecialRequirements = booking.SpecialRequirements,
                GuestInformation = guestInfo ?? new GuestInformationDto(),
                TotalPrice = booking.TotalPrice ?? 0,
                Approved = booking.Approved,
                BookingDate = booking.BookingDate,
                BookingStatus = booking.BookingStatus,
                ConfirmationNumber = booking.ConfirmationNumber
            };
        }
    }
}
