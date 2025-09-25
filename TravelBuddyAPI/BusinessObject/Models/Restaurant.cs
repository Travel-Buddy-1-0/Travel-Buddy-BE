using Newtonsoft.Json.Linq;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
namespace BusinessObject.Models
{
    [Table("Restaurant")]
    public class Restaurant : BaseModel
    {
        [Key]
        [Column("restaurant_id")]
        public int RestaurantId { get; set; }

        [Required, StringLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("average_price", TypeName = "decimal(10,2)")]
        public decimal? AveragePrice { get; set; }

        [Column("image", TypeName = "jsonb")]
        public JsonObject? Image { get; set; }

        [Column("style", TypeName = "jsonb")]
        public JsonObject? Style { get; set; }

        // Navigation properties
        public ICollection<Review> Reviews { get; set; }
        public ICollection<BookingDetail> BookingDetails { get; set; }
        public ICollection<Blog> Blogs { get; set; }
    }
}
