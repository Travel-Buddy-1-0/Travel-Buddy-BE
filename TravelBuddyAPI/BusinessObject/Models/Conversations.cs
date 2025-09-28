using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace BusinessObject.Models;

[Table("conversation")]
public class Conversation : BaseModel
{
    [PrimaryKey("conversation_id", false)]
    public int ConversationId { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }
}
