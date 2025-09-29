using System.ComponentModel.DataAnnotations;

namespace BusinessObject.DTOs
{
    public class HotelBookingRequestDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Hotel ID is required")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Check-in date is required")]
        public DateOnly CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required")]
        public DateOnly CheckOutDate { get; set; }

        [Required(ErrorMessage = "Number of nights is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of nights must be at least 1")]
        public int NumberOfNights { get; set; }

        [Required(ErrorMessage = "Number of guests is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of guests must be at least 1")]
        public int NumberOfGuests { get; set; }

        [Required(ErrorMessage = "Number of rooms is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of rooms must be at least 1")]
        public int NumberOfRooms { get; set; }

        [Required(ErrorMessage = "Guest arrival time is required")]
        public TimeOnly GuestArrivalTime { get; set; }

        [MaxLength(500, ErrorMessage = "Special requirements cannot exceed 500 characters")]
        public string? SpecialRequirements { get; set; }

        [Required(ErrorMessage = "Guest information is required")]
        public GuestInformationDto GuestInformation { get; set; } = null!;
    }

    public class GuestInformationDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string PhoneNumber { get; set; } = null!;

        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [MaxLength(50, ErrorMessage = "Nationality cannot exceed 50 characters")]
        public string? Nationality { get; set; }

        [MaxLength(20, ErrorMessage = "ID number cannot exceed 20 characters")]
        public string? IdNumber { get; set; }
    }
}
