using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace BusinessObject.Models;

[Table("service")]
public class Service : BaseModel
{
    [PrimaryKey("service_id", false)]
    public int ServiceId { get; set; }

    [Column("service_name")]
    public string ServiceName { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("price")]
    public decimal? Price { get; set; }

    [Column("type")]
    public string? Type { get; set; }

    [Column("provider_id")]
    public int? ProviderId { get; set; }
}
