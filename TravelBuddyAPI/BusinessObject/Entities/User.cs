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

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Bookingdetail> Bookingdetails { get; set; } = new List<Bookingdetail>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual ICollection<Useractivity> Useractivities { get; set; } = new List<Useractivity>();

    public virtual ICollection<Userpreference> Userpreferences { get; set; } = new List<Userpreference>();

    public virtual ICollection<CommentBlog> CommentBlogs { get; set; } = new List<CommentBlog>();
    public virtual ICollection<PaymentHistory> PaymentHistories { get; set; } = new List<PaymentHistory>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();


}
