using Newtonsoft.Json.Linq;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace BusinessObject.Models
{
    [Table("Users")]
    public class User : BaseModel
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required, StringLength(50)]
        [Column("username")]
        public string Username { get; set; }

        [Required, StringLength(255)]
        [Column("password")]
        public string Password { get; set; }

        [Required, StringLength(100)]
        [Column("email")]
        public string Email { get; set; }

        [Required, StringLength(100)]
        [Column("full_name")]
        public string FullName { get; set; }

        [StringLength(15)]
        [Column("phone_number")]
        public string? PhoneNumber { get; set; }

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("registration_date")]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [Column("photo", TypeName = "jsonb")]
        public JsonObject? Photo { get; set; }

        [Required, StringLength(20)]
        [Column("role")]
        public string Role { get; set; } = "user";

        // Navigation properties
        public ICollection<Review> Reviews { get; set; }
        public ICollection<BookingDetail> BookingDetails { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<Blog> Blogs { get; set; }
        public ICollection<UserActivity> UserActivities { get; set; }
        public ICollection<UserPreference> UserPreferences { get; set; }
        public ICollection<Participant> Participants { get; set; }
        public ICollection<Message> SentMessages { get; set; }
    }
}
