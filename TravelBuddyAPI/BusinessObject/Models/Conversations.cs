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
    [Table("Conversations")]
    public class Conversation : BaseModel 
    {
        [Key]
        [Column("conversation_id")]
        public int ConversationId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Participant> Participants { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
