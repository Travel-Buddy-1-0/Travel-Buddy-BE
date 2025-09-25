using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("Rooms")]
    public class Room : BaseModel
    {
        [Key]
        [Column("room_id")]
        public int RoomId { get; set; }

        [ForeignKey("Hotel")]
        [Column("hotel_id")]
        public int HotelId { get; set; }

        public Hotel Hotel { get; set; }

        [Required, StringLength(10)]
        [Column("room_number")]
        public string RoomNumber { get; set; }

        [Required, StringLength(50)]
        [Column("room_type")]
        public string RoomType { get; set; }

        [Column("price_per_night", TypeName = "decimal(10,2)")]
        public decimal PricePerNight { get; set; }

        [Column("capacity")]
        public int Capacity { get; set; }

        [Column("is_available")]
        public bool IsAvailable { get; set; } = true;

        // Navigation properties
    }
}
