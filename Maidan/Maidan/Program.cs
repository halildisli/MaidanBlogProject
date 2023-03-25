using Maidan.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Reflection;

namespace Maidan
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<MaidanDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("myHome")));

            builder.Services.AddIdentity<Author, IdentityRole>().AddEntityFrameworkStores<MaidanDbContext>().AddDefaultTokenProviders();

            //builder.Services.AddAuthentication(
            //    opt =>
            //    {
            //        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    }
            //    );

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());


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

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}