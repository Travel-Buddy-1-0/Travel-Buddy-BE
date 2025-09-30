using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Service
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public string? Type { get; set; }

    public int? ProviderId { get; set; }

    public virtual User? Provider { get; set; }
}
