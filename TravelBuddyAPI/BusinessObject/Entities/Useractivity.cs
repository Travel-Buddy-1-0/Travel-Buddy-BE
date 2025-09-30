using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Useractivity
{
    public int ActivityId { get; set; }

    public int? UserId { get; set; }

    public string? ActivityType { get; set; }

    public DateTime? ActivityDate { get; set; }

    public string? Metadata { get; set; }

    public virtual User? User { get; set; }
}
