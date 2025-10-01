using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Participant
{
    public int ParticipantId { get; set; }

    public int ConversationId { get; set; }

    public int UserId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
