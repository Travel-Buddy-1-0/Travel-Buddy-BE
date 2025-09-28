using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace BusinessObject.Models;

[Table("message")]
public class Message : BaseModel
{
    [PrimaryKey("message_id", false)]
    public int MessageId { get; set; }

    [Column("conversation_id")]
    public int ConversationId { get; set; }

    [Column("sender_id")]
    public int SenderId { get; set; }

    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("sent_at")]
    public DateTime? SentAt { get; set; }

    [Column("is_read")]
    public bool? IsRead { get; set; }
}
