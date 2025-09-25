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
    [Table("Locations")]
    public class Location : BaseModel
    {
        [Key]
        [Column("location_id")]
        public int LocationId { get; set; }

        [Required, StringLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("latitude", TypeName = "decimal(9,6)")]
        public decimal? Latitude { get; set; }

        [Column("longitude", TypeName = "decimal(9,6)")]
        public decimal? Longitude { get; set; }

        [StringLength(100)]
        [Column("country")]
        public string? Country { get; set; }

        [StringLength(100)]
        [Column("city")]
        public string? City { get; set; }

        [StringLength(100)]
        [Column("region")]
        public string? Region { get; set; }

        // Navigation properties
        public ICollection<Tour> Tours { get; set; }
    }
}
