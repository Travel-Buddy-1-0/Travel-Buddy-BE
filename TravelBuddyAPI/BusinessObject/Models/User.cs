using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System.Text.Json.Nodes;

namespace BusinessObject.Models;

[Table("users")]
public class User : BaseModel
{
    [PrimaryKey("user_id", false)]
    public int UserId { get; set; }

    [Column("username")]
    public string Username { get; set; } = null!;

    [Column("password")]
    public string Password { get; set; } = null!;

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("full_name")]
    public string FullName { get; set; } = null!;

    [Column("phone_number")]
    public string? PhoneNumber { get; set; }

    [Column("date_of_birth")]
    public DateOnly? DateOfBirth { get; set; }

    [Column("registration_date")]
    public DateTime? RegistrationDate { get; set; }

    [Column("photo")]
    public string? Photo { get; set; }

    [Column("role")]
    public string Role { get; set; } = null!;
    [Column("sex")]
    public string? Sex { get; set; }
}
