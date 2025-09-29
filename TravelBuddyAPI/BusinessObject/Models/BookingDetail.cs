using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Text.Json.Nodes;

namespace BusinessObject.Models
{
    [Table("bookingdetails")]
    public class BookingDetail : BaseModel
    {
        [PrimaryKey("booking_id", false)]
        public int BookingId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("tour_id")]
        public int? TourId { get; set; }

        [Column("hotel_id")]
        public int? HotelId { get; set; }

        [Column("restaurant_id")]
        public int? RestaurantId { get; set; }

        [Column("booking_date")]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Column("check_in_date")]
        public DateOnly? CheckInDate { get; set; }

        [Column("check_out_date")]
        public DateOnly? CheckOutDate { get; set; }

        [Column("number_of_nights")]
        public int? NumberOfNights { get; set; }

        [Column("number_of_guests")]
        public int? NumberOfGuests { get; set; }

        [Column("number_of_rooms")]
        public int? NumberOfRooms { get; set; }

        [Column("guest_arrival_time")]
        public TimeOnly? GuestArrivalTime { get; set; }

        [Column("special_requirements")]
        public string? SpecialRequirements { get; set; }

        [Column("guest_information")]
        public JsonObject? GuestInformation { get; set; }

        [Column("total_price")]
        public decimal? TotalPrice { get; set; }

        [Column("approved")]
        public bool Approved { get; set; } = false;

        [Column("confirmation_number")]
        public string? ConfirmationNumber { get; set; }

        [Column("booking_status")]
        public string BookingStatus { get; set; } = "Pending";
    }
}
