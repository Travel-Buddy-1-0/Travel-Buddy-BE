using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Template
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? DefaultLayout { get; set; }

    public string? PreviewImage { get; set; }

    public string? StyleSchema { get; set; }

    public bool? IsPremium { get; set; }

    public string? DefaultDataJson { get; set; }

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();
}
