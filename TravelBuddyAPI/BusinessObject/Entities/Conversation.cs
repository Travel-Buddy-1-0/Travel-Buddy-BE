using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Conversation
{
    public int ConversationId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
}
