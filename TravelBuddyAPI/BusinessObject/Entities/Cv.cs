using BusinessObject.Enum;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace BusinessObject.Entities;

public partial class Cv
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? TemplateId { get; set; }

    public string? Title { get; set; }

    public string? Slug { get; set; }

    public bool? IsPublished { get; set; }

    public string? ThumbnailUrl { get; set; }

    public JsonObject? LayoutConfig { get; set; }

    public JsonObject? StyleConfig { get; set; }

    public JsonObject? DataJson { get; set; }
    public CreationSource CreationSource { get; set; }
    public ProcessingStatus ProcessingStatus { get; set; }
    public string? CvRawData { get; set; }

    public string? JdRawData { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public decimal? Version { get; set; }
    public virtual User? User { get; set; }
    public virtual Template? Template { get; set; }
}
