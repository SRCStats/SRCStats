using System.Text.Json;

using SRCStats.Data;
using SRCStats.Hubs;
using SRCStats.Models;
using SRCStats.Models.SRC;

using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace SRCStats
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<StatsDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("SRC_STATS_SQL_CONNECTION_STRING") ?? throw new ArgumentNullException("SQL connection string wasn't provided!")));
            builder.Services.Configure<WebhookDbContext>(options => options.ConnectionString = Environment.GetEnvironmentVariable("SRC_STATS_MONGODB_CONNECTION_STRING") ?? throw new ArgumentNullException("MongoDB connection string wasn't provided!"));
            builder.Services.AddSignalR(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(120);
                options.KeepAliveInterval = TimeSpan.FromSeconds(40);
                options.MaximumParallelInvocationsPerClient = 10;
            });
            builder.Services.AddSingleton<WebhookDbService>();
            builder.Services.AddMemoryCache();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapHub<UserHub>("/userHub");
            app.MapHub<WebhookHub>("/webhookHub");

            var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<StatsDbContext>();

            if (db.Database.GetPendingMigrations().Any())
                db.Database.Migrate();

            Archetypes.InitializeArchetypes(db);
            Trophies.InitializeTrophies(db);
            db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE InProgress");

            app.Run();

        }
    }
}