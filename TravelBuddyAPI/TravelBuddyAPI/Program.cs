
using BusinessLogic.Services;
using BusinessObject.Data;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.Services;
using Repositories;
using Supabase;

namespace TravelBuddyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Sockets.UseOnlyIPv4Stack", true);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);            
            var builder = WebApplication.CreateBuilder(args);

            // Add Supabase for authentication only
            // --- Supabase config ---
            var url = builder.Configuration["Supabase:Url"];
            var key = builder.Configuration["Authentication:Key"];
            var options = new Supabase.SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };
            builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));

            // Add Entity Framework
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("MyCnn")));

            // Add services to the container.

            // --- Services ---
            builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IHotelRepository, HotelRepository>();


            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IHotelService, HotelService>();
            builder.Services.AddControllers();

            // --- CORS ---
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()    // Cho phép mọi origin (cẩn thận khi deploy prod)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // --- JWT Authentication ---
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
                        ValidAudience = builder.Configuration["Jwt:ValidAudience"],
                        IssuerSigningKey =
                            new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            // --- Swagger ---
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // --- Middleware ---
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");       

            app.UseAuthentication();     
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
