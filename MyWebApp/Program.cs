using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Repository;
using MyWebApp.Models;
using MyWebApp.Extensions;

namespace MyWebApp
{
    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLogging(configure =>
            {
                configure.AddFile($"log file {DateTime.Now.GetMyTimeFormat()}.txt");
                configure.AddConsole();
                configure.AddDebug();
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<RandomGenerator>();
            builder.Services.AddScoped<PicturesLoader>();
            builder.Services.AddScoped<CredentialsRepository>();
            builder.Services.AddScoped<NotesRepository>();
            builder.Services.AddScoped<UsersRepository>();
            builder.Services.AddScoped<ThreadsRepository>();
            builder.Services.AddScoped<DashboardRepository>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddIdentity<UserModel, IdentityRole>(opt =>
            {
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.,-+()=";
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredUniqueChars = 0;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            var app = builder.Build();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            if (args.Length > 0 &&
                args[0].ToLower() == "seeddata")
            {
                Console.WriteLine("(Main) Seeding the database");

                var seedUsersModel = await Seed.SeedUsersAndRolesAsync(app);
                await Seed.SeedData(app, seedUsersModel.Admin, seedUsersModel.Users);
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseExceptionHandler("/Error/Error500");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.Use(async (ctx, next) =>
            {
                await next();

                if (ctx.Response.StatusCode == 404 &&
                    !ctx.Response.HasStarted)
                {
                    // Re-execute the request so the user gets the error page
                    var originalPath = ctx.Request.Path.Value;
                    if (originalPath != null)
                    {
                        ctx.Items["originalPath"] = originalPath;
                    }
                    ctx.Request.Path = "/Error/Error404";

                    await next();
                }
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();
        }
    }
}