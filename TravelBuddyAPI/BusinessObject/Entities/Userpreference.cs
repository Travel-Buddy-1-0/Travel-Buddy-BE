using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Userpreference
{
    public int PreferenceId { get; set; }

    public int? UserId { get; set; }

    public string? Travelstyle { get; set; }

    public decimal? Budget { get; set; }

    public string? Cuisine { get; set; }

    public string? PreferenceType { get; set; }

    public string? Destionation { get; set; }

    public virtual User? User { get; set; }
}
