using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace BusinessObject.Models;

[Table("participant")]
public class Participant : BaseModel
{
    [PrimaryKey("participant_id", false)]
    public int ParticipantId { get; set; }

    [Column("conversation_id")]
    public int ConversationId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("joined_at")]
    public DateTime? JoinedAt { get; set; }
}
