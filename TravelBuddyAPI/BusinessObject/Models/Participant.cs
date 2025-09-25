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
    [Table("Participants")]
    public class Participant : BaseModel
    {
        [Key]
        [Column("participant_id")]
        public int ParticipantId { get; set; }

        [ForeignKey("Conversation")]
        [Column("conversation_id")]
        public int ConversationId { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("joined_at")]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Conversation Conversation { get; set; }
        public User User { get; set; }
    }
}
