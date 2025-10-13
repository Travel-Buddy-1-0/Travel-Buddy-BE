using BusinessLogic.Exceptions;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using Repositories;
using System.Text.Json;

namespace BusinessLogic.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllReviewsAsync();
            return reviews.Select(MapToDto).ToList();
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int reviewId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            return review != null ? MapToDto(review) : null;
        }

        public async Task<List<ReviewDto>> GetReviewsByUserIdAsync(int userId)
        {
            var reviews = await _reviewRepository.GetReviewsByUserIdAsync(userId);
            return reviews.Select(MapToDto).ToList();
        }

        public async Task<List<ReviewDto>> GetReviewsByHotelIdAsync(int hotelId)
        {
            var reviews = await _reviewRepository.GetReviewsByHotelIdAsync(hotelId);
            return reviews.Select(MapToDto).ToList();
        }

        public async Task<List<ReviewDto>> GetReviewsByRestaurantIdAsync(int restaurantId)
        {
            var reviews = await _reviewRepository.GetReviewsByRestaurantIdAsync(restaurantId);
            return reviews.Select(MapToDto).ToList();
        }

        public async Task<List<ReviewDto>> GetReviewsByTourIdAsync(int tourId)
        {
            var reviews = await _reviewRepository.GetReviewsByTourIdAsync(tourId);
            return reviews.Select(MapToDto).ToList();
        }

        public async Task<List<ReviewDto>> GetReviewsByFilterAsync(ReviewFilterRequestDto filter)
        {
            var reviews = await _reviewRepository.GetReviewsByFilterAsync(
                filter.UserId,
                filter.TourId,
                filter.HotelId,
                filter.RestaurantId,
                filter.Rating,
                filter.FromDate,
                filter.ToDate,
                filter.Limit,
                filter.Offset
            );
            return reviews.Select(MapToDto).ToList();
        }

        public async Task<ReviewDto> CreateReviewAsync(ReviewCreateRequestDto request)
        {
            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5.");
            }

            // Validate that at least one entity is specified
            if (!request.TourId.HasValue && !request.HotelId.HasValue && !request.RestaurantId.HasValue)
            {
                throw new ArgumentException("At least one entity (Tour, Hotel, or Restaurant) must be specified.");
            }

            // Check if user already has review for this entity
            var exists = await _reviewRepository.ExistsReviewByUserAndEntityAsync(
                request.UserId ?? 0, 
                request.TourId, 
                request.HotelId, 
                request.RestaurantId
            );
            if (exists)
            {
                throw new ConflictException("User has already provided a review for this entity.");
            }

            var review = new Review
            {
                UserId = request.UserId,
                TourId = request.TourId,
                HotelId = request.HotelId,
                RestaurantId = request.RestaurantId,
                Rating = request.Rating,
                Comment = request.Comment,
                Image = ConvertStringToJson(request.Image)
            };

            var createdReview = await _reviewRepository.AddReviewAsync(review);
            return MapToDto(createdReview);
        }

        public async Task<ReviewDto> UpdateReviewAsync(int reviewId, ReviewUpdateRequestDto request)
        {
            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5.");
            }

            var existingReview = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (existingReview == null)
            {
                throw new NotFoundException($"Review with ID {reviewId} not found.");
            }

            existingReview.Rating = request.Rating;
            existingReview.Comment = request.Comment;
            existingReview.Image = request.Image != null ? ConvertStringToJson(request.Image) : existingReview.Image;

            var updatedReview = await _reviewRepository.UpdateReviewAsync(existingReview);
            return MapToDto(updatedReview);
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var existingReview = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (existingReview == null)
            {
                throw new NotFoundException($"Review with ID {reviewId} not found.");
            }

            await _reviewRepository.DeleteReviewAsync(reviewId);
        }

        public async Task<bool> CanUserCreateReviewAsync(int userId, int? tourId, int? hotelId, int? restaurantId)
        {
            return !await _reviewRepository.ExistsReviewByUserAndEntityAsync(userId, tourId, hotelId, restaurantId);
        }

        private static ReviewDto MapToDto(Review review)
        {
            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                UserId = review.UserId,
                TourId = review.TourId,
                HotelId = review.HotelId,
                RestaurantId = review.RestaurantId,
                Rating = review.Rating,
                Comment = review.Comment,
                Image = review.Image,
                ReviewDate = review.ReviewDate,
                UserName = review.User?.FullName,
                HotelName = review.Hotel?.Name,
                RestaurantName = review.Restaurant?.Name,
                TourName = review.Tour?.Title
            };
        }

        private static string? ConvertStringToJson(string? image)
        {
            if (string.IsNullOrEmpty(image))
                return null;

            try
            {
                // Try to parse as JSON first
                JsonDocument.Parse(image);
                return image; // Already valid JSON
            }
            catch
            {
                // If not valid JSON, wrap it in a JSON array
                var jsonArray = new[] { image };
                return JsonSerializer.Serialize(jsonArray);
            }
        }
    }
}
