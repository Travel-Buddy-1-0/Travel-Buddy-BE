using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

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

        [Column("total_price")]
        public decimal? TotalPrice { get; set; }

        [Column("approved")]
        public bool Approved { get; set; } = false;
    }
}
