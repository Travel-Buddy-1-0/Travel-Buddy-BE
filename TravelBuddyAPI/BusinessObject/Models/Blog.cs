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
    [Table("Blog")]
    public class Blog : BaseModel
    {
        [Key]
        [Column("blog_id")]
        public int BlogId { get; set; }

        [Required, StringLength(200)]
        [Column("title")]
        public string Title { get; set; }

        [Column("content")]
        public string? Content { get; set; }

        [Column("publish_date")]
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;

        [Column("image", TypeName = "jsonb")]
        public JsonObject? Image { get; set; }

        [ForeignKey("Author")]
        [Column("author_id")]
        public int? AuthorId { get; set; }

        [ForeignKey("Hotel")]
        [Column("hotel_id")]
        public int? HotelId { get; set; }

        [ForeignKey("Restaurant")]
        [Column("restaurant_id")]
        public int? RestaurantId { get; set; }

        // Navigation properties
        public User Author { get; set; }
        public Hotel Hotel { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}
