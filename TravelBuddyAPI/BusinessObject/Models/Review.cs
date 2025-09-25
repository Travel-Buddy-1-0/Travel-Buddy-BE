using Newtonsoft.Json.Linq;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
namespace BusinessObject.Models
{
    [Table("Reviews")]
    public class Review : BaseModel
    {
        [Key]
        [Column("review_id")]
        public int ReviewId { get; set; }

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

        [Range(1, 5)]
        [Column("rating")]
        public int Rating { get; set; }

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("image", TypeName = "jsonb")]
        public JsonObject? Image { get; set; }

        [Column("review_date")]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; }
        public Tour Tour { get; set; }
        public Hotel Hotel { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}
