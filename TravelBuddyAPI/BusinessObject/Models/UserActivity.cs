using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System.Text.Json.Nodes;

namespace BusinessObject.Models;

[Table("useractivity")]
public class UserActivity : BaseModel
{
    [PrimaryKey("activity_id", false)]
    public int ActivityId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("activity_type")]
    public string? ActivityType { get; set; }

    [Column("activity_date")]
    public DateTime? ActivityDate { get; set; }

    [Column("metadata")]
    public JsonObject? Metadata { get; set; }
}
