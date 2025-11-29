using BusinessObject.Data;
using BusinessObject.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Implements;
using Repositories.Interfaces;
using Services.Implement;
using Services.Interfaces;

using TravelBuddyAPI.Models;

namespace TravelBuddyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Sockets.UseOnlyIPv4Stack", true);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>(optional: true);
            }
            //var configuration = builder.Configuration.Get<Config>() ?? new Config();
            //builder.Services.AddSingleton(configuration);
            //if(configuration.Provider == "Gemini")
            //{

            //}

   
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
            builder.Services.Configure<PayOsSettings>(builder.Configuration.GetSection("PayOS"));
         
            builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));
      
  

            builder.Services.AddScoped<IUserRepository, UserRepository>();
 

            builder.Services.AddScoped<IUserService, UserService>();
            // HttpClient cho SupabaseService
            builder.Services.AddHttpClient<SupabaseService>();
            builder.Services.AddHttpClient<GeminiService>();
            // RedisService singleton
            builder.Services.AddSingleton<RedisService>();
            builder.Services.AddScoped<FileParserService>();
            builder.Services.AddScoped<PuppeteerSharpPDFServices>();
            // JobQueueService singleton


            builder.Services.AddScoped<QuestPdfService>();
            // Đăng ký Service của bạn

            builder.Services.AddSingleton<JobQueueService>();

            // Worker background
            builder.Services.AddHostedService<CvWorker>();
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