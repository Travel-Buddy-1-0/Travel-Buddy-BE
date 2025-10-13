using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Blog
{
    public int BlogId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime? PublishDate { get; set; }

    public string? Image { get; set; }

    public int? AuthorId { get; set; }

    public int? HotelId { get; set; }

    public int? RestaurantId { get; set; }

    public string? Tags { get; set; }

    public virtual User? Author { get; set; }

    public virtual Hotel? Hotel { get; set; }

    public virtual Restaurant? Restaurant { get; set; }

}
