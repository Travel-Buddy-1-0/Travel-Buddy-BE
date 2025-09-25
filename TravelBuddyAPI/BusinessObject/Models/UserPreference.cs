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
    [Table("UserPreference")]
    public class UserPreference : BaseModel
    {
        [Key]
        [Column("preference_id")]
        public int PreferenceId { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }

        [StringLength(100)]
        [Column("TravelStyle")]
        public string? TravelStyle { get; set; }

        [Column("Budget", TypeName = "numeric(12,2)")]
        public decimal? Budget { get; set; }

        [StringLength(100)]
        [Column("Cuisine")]
        public string? Cuisine { get; set; }

        [StringLength(50)]
        [Column("preference_type")]
        public string? PreferenceType { get; set; }

        [Column("FavouriteActivities", TypeName = "jsonb")]
        public JsonObject? FavouriteActivities { get; set; }

        // Navigation properties
        public User User { get; set; }
    }
}
