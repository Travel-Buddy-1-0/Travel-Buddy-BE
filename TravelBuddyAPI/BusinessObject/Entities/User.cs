

using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public string? Photo { get; set; }

    public string? Role { get; set; }

    public string? Sex { get; set; }
    public decimal? WalletBalance { get; set; }




    public virtual ICollection<Cv> CVs { get; set; } = new List<Cv>();

}
