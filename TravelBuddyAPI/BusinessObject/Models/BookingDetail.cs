using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("BookingDetails")]
    public class BookingDetail : BaseModel
    {
        [Key]
        [Column("booking_id")]
        public int BookingId { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("Tour")]
        [Column("tour_id")]
        public int? TourId { get; set; }

        [ForeignKey("Hotel")]
        [Column("hotel_id")]
        public int? HotelId { get; set; }

        [ForeignKey("Restaurant")]
        [Column("restaurant_id")]
        public int? RestaurantId { get; set; }

        [Column("booking_date")]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Column("check_in_date")]
        public DateTime? CheckInDate { get; set; }

        [Column("check_out_date")]
        public DateTime? CheckOutDate { get; set; }

        [Column("total_price", TypeName = "decimal(10,2)")]
        public decimal? TotalPrice { get; set; }

        [Column("approved")]
        public bool Approved { get; set; } = false;

        // Navigation properties
        public User User { get; set; }
        public Tour Tour { get; set; }
        public Hotel Hotel { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}
