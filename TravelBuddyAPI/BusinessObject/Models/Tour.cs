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
    [Table("Tours")]
    public class Tour : BaseModel
    {
        [Key]
        [Column("tour_id")]
        public int TourId { get; set; }

        [Required, StringLength(100)]
        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("duration")]
        public int? Duration { get; set; }

        [Column("price", TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }

        [ForeignKey("Location")]
        [Column("location_id")]
        public int? LocationId { get; set; }

        public Location? Location { get; set; }

        // Navigation properties
        
        public ICollection<Review> Reviews { get; set; }
        public ICollection<BookingDetail> BookingDetails { get; set; }
    }
}
