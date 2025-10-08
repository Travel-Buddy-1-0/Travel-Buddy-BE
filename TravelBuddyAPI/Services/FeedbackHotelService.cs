using BusinessLogic.Exceptions;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using Repositories;

namespace BusinessLogic.Services
{
    public class FeedbackHotelService : IFeedbackHotelService
    {
        private readonly IFeedbackHotelRepository _feedbackHotelRepository;

        public FeedbackHotelService(IFeedbackHotelRepository feedbackHotelRepository)
        {
            _feedbackHotelRepository = feedbackHotelRepository;
        }

        public async Task<List<FeedbackHotelDto>> GetAllFeedbackHotelsAsync()
        {
            var feedbackHotels = await _feedbackHotelRepository.GetAllFeedbackHotelsAsync();
            return feedbackHotels.Select(MapToDto).ToList();
        }

        public async Task<FeedbackHotelDto?> GetFeedbackHotelByIdAsync(int feedbackId)
        {
            var feedbackHotel = await _feedbackHotelRepository.GetFeedbackHotelByIdAsync(feedbackId);
            return feedbackHotel != null ? MapToDto(feedbackHotel) : null;
        }

        public async Task<List<FeedbackHotelDto>> GetFeedbackHotelsByUserIdAsync(int userId)
        {
            var feedbackHotels = await _feedbackHotelRepository.GetFeedbackHotelsByUserIdAsync(userId);
            return feedbackHotels.Select(MapToDto).ToList();
        }

        public async Task<List<FeedbackHotelDto>> GetFeedbackHotelsByHotelIdAsync(int hotelId)
        {
            var feedbackHotels = await _feedbackHotelRepository.GetFeedbackHotelsByHotelIdAsync(hotelId);
            return feedbackHotels.Select(MapToDto).ToList();
        }

        public async Task<List<FeedbackHotelDto>> GetFeedbackHotelsByFilterAsync(FeedbackHotelFilterRequestDto filter)
        {
            var feedbackHotels = await _feedbackHotelRepository.GetFeedbackHotelsByFilterAsync(
                filter.UserId,
                filter.HotelId,
                filter.Rating,
                filter.FromDate,
                filter.ToDate,
                filter.Limit,
                filter.Offset
            );
            return feedbackHotels.Select(MapToDto).ToList();
        }

        public async Task<FeedbackHotelDto> CreateFeedbackHotelAsync(FeedbackHotelCreateRequestDto request)
        {
            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5.");
            }

            // Check if user already has feedback for this hotel
            var exists = await _feedbackHotelRepository.ExistsFeedbackByUserAndHotelAsync(request.UserId, request.HotelId);
            if (exists)
            {
                throw new ConflictException("User has already provided feedback for this hotel.");
            }

            var feedbackHotel = new FeedbackHotel
            {
                UserId = request.UserId,
                HotelId = request.HotelId,
                Rating = request.Rating,
                Comment = request.Comment
            };

            var createdFeedback = await _feedbackHotelRepository.AddFeedbackHotelAsync(feedbackHotel);
            return MapToDto(createdFeedback);
        }

        public async Task<FeedbackHotelDto> UpdateFeedbackHotelAsync(int feedbackId, FeedbackHotelUpdateRequestDto request)
        {
            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5.");
            }

            var existingFeedback = await _feedbackHotelRepository.GetFeedbackHotelByIdAsync(feedbackId);
            if (existingFeedback == null)
            {
                throw new NotFoundException($"FeedbackHotel with ID {feedbackId} not found.");
            }

            existingFeedback.Rating = request.Rating;
            existingFeedback.Comment = request.Comment;

            var updatedFeedback = await _feedbackHotelRepository.UpdateFeedbackHotelAsync(existingFeedback);
            return MapToDto(updatedFeedback);
        }

        public async Task DeleteFeedbackHotelAsync(int feedbackId)
        {
            var existingFeedback = await _feedbackHotelRepository.GetFeedbackHotelByIdAsync(feedbackId);
            if (existingFeedback == null)
            {
                throw new NotFoundException($"FeedbackHotel with ID {feedbackId} not found.");
            }

            await _feedbackHotelRepository.DeleteFeedbackHotelAsync(feedbackId);
        }

        public async Task<bool> CanUserCreateFeedbackAsync(int userId, int hotelId)
        {
            return !await _feedbackHotelRepository.ExistsFeedbackByUserAndHotelAsync(userId, hotelId);
        }

        private static FeedbackHotelDto MapToDto(FeedbackHotel feedbackHotel)
        {
            return new FeedbackHotelDto
            {
                FeedbackId = feedbackHotel.FeedbackId,
                UserId = feedbackHotel.UserId,
                HotelId = feedbackHotel.HotelId,
                Rating = feedbackHotel.Rating,
                Comment = feedbackHotel.Comment,
                CreatedAt = feedbackHotel.CreatedAt,
                UpdatedAt = feedbackHotel.UpdatedAt,
                UserName = feedbackHotel.User?.FullName,
                HotelName = feedbackHotel.Hotel?.Name
            };
        }
    }
}
