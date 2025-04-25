using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamDaoTao.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using QuanLyTrungTamDaoTao.Models;
namespace QuanLyTrungTamDaoTao
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<QuanLyTrungTamDaoTaoContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DataBase")));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options =>
                {
                    options.LoginPath = "/Account/DangNhap";
                    options.LogoutPath = "/Account/DangXuat";
                    options.AccessDeniedPath = "/AccessDenied";
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("QTV"));
                options.AddPolicy("HocVien", policy => policy.RequireRole("HV"));
            });

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "Admin",
                pattern: "{area:exists}/{controller=HomeAdmin}/{action=Dashboard}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
