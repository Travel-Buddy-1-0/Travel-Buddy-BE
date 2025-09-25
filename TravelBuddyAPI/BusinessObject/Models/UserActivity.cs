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
    [Table("UserActivity")]
    public class UserActivity : BaseModel
    {
        [Key]
        [Column("activity_id")]
        public int ActivityId { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("activity_type")]
        [StringLength(50)]
        public string? ActivityType { get; set; }

        [Column("activity_date")]
        public DateTime ActivityDate { get; set; } = DateTime.UtcNow;

        [Column("Metadata", TypeName = "jsonb")]
        public JsonObject? Metadata { get; set; }
    }
}
