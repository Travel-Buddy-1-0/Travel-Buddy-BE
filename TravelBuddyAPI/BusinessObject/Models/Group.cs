using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace BusinessObject.Models;

[Table("group")]
public class Group : BaseModel
{
    [PrimaryKey("group_id", false)]
    public int GroupId { get; set; }

    [Column("group_name")]
    public string GroupName { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("createdat")]
    public DateTime? CreatedAt { get; set; }
}
