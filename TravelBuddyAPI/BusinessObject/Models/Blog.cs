using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Nodes;

namespace BusinessObject.Models
{
    [Table("blog")]
    public class Blog : BaseModel
    {
        [PrimaryKey("blog_id", false)]  // false = Postgres tự sinh (IDENTITY)
        public int BlogId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("content")]
        public string? Content { get; set; }

        [Column("publish_date")]
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;

        [Column("image")]
        public JsonObject? Image { get; set; }

        [Column("author_id")]
        public int? AuthorId { get; set; }

        [Column("hotel_id")]
        public int? HotelId { get; set; }

        [Column("restaurant_id")]
        public int? RestaurantId { get; set; }
        [Column("tags")]
        public string? Tags { get; set; }
    }
}
