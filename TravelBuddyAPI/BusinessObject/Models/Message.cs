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
    [Table("Messages")]
    public class Message : BaseModel
    {
        [Key]
        [Column("message_id")]
        public int MessageId { get; set; }

        [ForeignKey("Conversation")]
        [Column("conversation_id")]
        public int ConversationId { get; set; }

        [ForeignKey("Sender")]
        [Column("sender_id")]
        public int SenderId { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; }

        [Column("sent_at")]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [Column("is_read")]
        public bool IsRead { get; set; } = false;

        // Navigation properties
        public Conversation Conversation { get; set; }
        public User Sender { get; set; }
    }
}
