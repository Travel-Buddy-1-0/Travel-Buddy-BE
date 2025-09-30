using System;
using System.Collections.Generic;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessObject.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Bookingdetail> Bookingdetails { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Useractivity> Useractivities { get; set; }

    public virtual DbSet<Userpreference> Userpreferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=db.jjcpfegjaqefkmwenosq.supabase.co;Database=postgres;Username=postgres;Password=Travel@buddy.1734;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("unaccent")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("blog_pkey");

            entity.ToTable("blog");

            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.Image)
                .HasColumnType("jsonb")
                .HasColumnName("image");
            entity.Property(e => e.PublishDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("publish_date");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");
            entity.Property(e => e.Tags)
                .HasColumnType("jsonb")
                .HasColumnName("tags");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("blog_author_id_fkey");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("blog_hotel_id_fkey");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("blog_restaurant_id_fkey");
        });

        modelBuilder.Entity<Bookingdetail>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("bookingdetails_pkey");

            entity.ToTable("bookingdetails");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Approved)
                .HasDefaultValue(false)
                .HasColumnName("approved");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("booking_date");
            entity.Property(e => e.CheckInDate).HasColumnName("check_in_date");
            entity.Property(e => e.CheckOutDate).HasColumnName("check_out_date");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.TourId).HasColumnName("tour_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Bookingdetails)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bookingdetails_hotel_id_fkey");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Bookingdetails)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bookingdetails_restaurant_id_fkey");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookingdetails)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("bookingdetails_room_id_fkey");

            entity.HasOne(d => d.Tour).WithMany(p => p.Bookingdetails)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bookingdetails_tour_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Bookingdetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bookingdetails_user_id_fkey");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId).HasName("conversations_pkey");

            entity.ToTable("conversations");

            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("groups_pkey");

            entity.ToTable("groups");

            entity.HasIndex(e => e.GroupName, "groups_group_name_key").IsUnique();

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .HasColumnName("group_name");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.HotelId).HasName("hotel_pkey");

            entity.ToTable("hotel");

            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Image)
                .HasColumnType("jsonb")
                .HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Style)
                .HasColumnType("jsonb")
                .HasColumnName("style");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("locations_pkey");

            entity.ToTable("locations");

            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Latitude)
                .HasPrecision(9, 6)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(9, 6)
                .HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Region)
                .HasMaxLength(100)
                .HasColumnName("region");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("messages_pkey");

            entity.ToTable("messages");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("sent_at");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .HasConstraintName("messages_conversation_id_fkey");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("messages_sender_id_fkey");
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasKey(e => e.ParticipantId).HasName("participants_pkey");

            entity.ToTable("participants");

            entity.HasIndex(e => new { e.ConversationId, e.UserId }, "participants_conversation_id_user_id_key").IsUnique();

            entity.Property(e => e.ParticipantId).HasColumnName("participant_id");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("joined_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Participants)
                .HasForeignKey(d => d.ConversationId)
                .HasConstraintName("participants_conversation_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Participants)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("participants_user_id_fkey");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.RestaurantId).HasName("restaurant_pkey");

            entity.ToTable("restaurant");

            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AveragePrice)
                .HasPrecision(10, 2)
                .HasColumnName("average_price");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Image)
                .HasColumnType("jsonb")
                .HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Style)
                .HasColumnType("jsonb")
                .HasColumnName("style");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("reviews_pkey");

            entity.ToTable("reviews");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.Image)
                .HasColumnType("jsonb")
                .HasColumnName("image");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");
            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("review_date");
            entity.Property(e => e.TourId).HasColumnName("tour_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("reviews_hotel_id_fkey");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("reviews_restaurant_id_fkey");

            entity.HasOne(d => d.Tour).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("reviews_tour_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("reviews_user_id_fkey");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("rooms_pkey");

            entity.ToTable("rooms");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.Image)
                .HasColumnType("jsonb")
                .HasColumnName("image");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("is_available");
            entity.Property(e => e.PricePerNight)
                .HasPrecision(10, 2)
                .HasColumnName("price_per_night");
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(10)
                .HasColumnName("room_number");
            entity.Property(e => e.RoomType)
                .HasMaxLength(50)
                .HasColumnName("room_type");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("rooms_hotel_id_fkey");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("services_pkey");

            entity.ToTable("services");

            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .HasColumnName("service_name");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.Provider).WithMany(p => p.Services)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("services_provider_id_fkey");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.TourId).HasName("tours_pkey");

            entity.ToTable("tours");

            entity.Property(e => e.TourId).HasColumnName("tour_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");

            entity.HasOne(d => d.Location).WithMany(p => p.Tours)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("tours_location_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
            entity.Property(e => e.Photo)
                .HasColumnType("character varying")
                .HasColumnName("photo");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("registration_date");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValueSql("'user'::character varying")
                .HasColumnName("role");
            entity.Property(e => e.Sex)
                .HasColumnType("character varying")
                .HasColumnName("sex");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Useractivity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("useractivity_pkey");

            entity.ToTable("useractivity");

            entity.Property(e => e.ActivityId).HasColumnName("activity_id");
            entity.Property(e => e.ActivityDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("activity_date");
            entity.Property(e => e.ActivityType)
                .HasMaxLength(50)
                .HasColumnName("activity_type");
            entity.Property(e => e.Metadata)
                .HasColumnType("jsonb")
                .HasColumnName("metadata");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Useractivities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("useractivity_user_id_fkey");
        });

        modelBuilder.Entity<Userpreference>(entity =>
        {
            entity.HasKey(e => e.PreferenceId).HasName("userpreference_pkey");

            entity.ToTable("userpreference");

            entity.Property(e => e.PreferenceId).HasColumnName("preference_id");
            entity.Property(e => e.Budget)
                .HasPrecision(12, 2)
                .HasColumnName("budget");
            entity.Property(e => e.Cuisine).HasColumnName("cuisine");
            entity.Property(e => e.Destionation)
                .HasColumnType("jsonb")
                .HasColumnName("destionation");
            entity.Property(e => e.PreferenceType)
                .HasMaxLength(50)
                .HasColumnName("preference_type");
            entity.Property(e => e.Travelstyle)
                .HasColumnType("jsonb")
                .HasColumnName("travelstyle");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Userpreferences)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("userpreference_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
