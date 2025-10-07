using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Group
{
    public int GroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? Createdat { get; set; }
}
