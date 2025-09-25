using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BusinessObject.Models
{
    [Table("Services")]
    public class Service : BaseModel
    {
        [Key]
        [Column("service_id")]
        public int ServiceId { get; set; }

        [Required, StringLength(100)]
        [Column("service_name")]
        public string ServiceName { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("price", TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }

        [StringLength(50)]
        [Column("type")]
        public string? Type { get; set; }

        [ForeignKey("Provider")]
        [Column("provider_id")]
        public int? ProviderId { get; set; }

        public User? Provider { get; set; }

        // Navigation properties
    }
}
