namespace BusinessObject.DTOs
{
    public class HotelBookingResponseDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = null!;
        public string HotelAddress { get; set; } = null!;
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int NumberOfNights { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfRooms { get; set; }
        public TimeOnly GuestArrivalTime { get; set; }
        public string? SpecialRequirements { get; set; }
        public GuestInformationDto GuestInformation { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public bool Approved { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingStatus { get; set; } = null!;
        public string? ConfirmationNumber { get; set; }
    }

    public class BookingConfirmationDto
    {
        public int BookingId { get; set; }
        public string ConfirmationNumber { get; set; } = null!;
        public string HotelName { get; set; } = null!;
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int NumberOfNights { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal TotalPrice { get; set; }
        public string GuestName { get; set; } = null!;
        public string GuestEmail { get; set; } = null!;
        public string GuestPhone { get; set; } = null!;
        public DateTime BookingDate { get; set; }
        public string BookingStatus { get; set; } = null!;
    }
}
