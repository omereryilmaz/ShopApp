using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopApp.Business.Abstract;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Abstract;
using ShopApp.DataAccess.Concrete.EfCore;
using ShopApp.WebUI.EmailServices;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.Middlewares;

namespace ShopApp.WebUI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("IdentityCOnnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options => {
                // Password Ayarlari
                // Mutlaka icerisinde sayisal deger ister
                options.Password.RequireDigit = true;
                // Mutlaka 1 tane kucuk karakter ister
                options.Password.RequireLowercase = true;
                // Mutlaka 1 tane buyuk karakter ister
                options.Password.RequireUppercase = true;
                // Minimum 6 karakterli
                options.Password.RequiredLength = 6;

                // 5 kere yanlis girmek hakki    
                options.Lockout.MaxFailedAccessAttempts = 5;
                // 5 kere yanlis girerse kullanciyi 5dk kilitle
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                // Kilitleme islemi yeni kullanici icin de gecerli
                options.Lockout.AllowedForNewUsers = true;

                // Daha once kayitli maile tekrar izin verme
                options.User.RequireUniqueEmail = true;
                // true: Kayittan sonra maili onaylamasini zorunlu kil
                options.SignIn.RequireConfirmedEmail = true;
            });

            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                // Kullanici erisiminin olmamasının istenildiginde yonlendirir
                options.AccessDeniedPath = "/account/accesdenied";

                // Cookie suresi 60 dk olarak ayarlandi
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

                // true: cookie suresi bitinci login islemi gerektirir
                // false: kullanici aktifse cookie suresini uzatir
                options.SlidingExpiration = true;

                options.Cookie = new CookieBuilder { 
                    // sciptler ile cookie'lerin okunmasini engeller
                    // sadece http cagrilari ile okunur
                    HttpOnly = true,
                    //Cookie ismi
                    Name = ".ShopApp.Security.Cookie",
                    SameSite = SameSiteMode.Strict
                };
            });


            services.AddControllersWithViews();          

            services.AddScoped<IProductDal, EfCoreProductDal>();
            services.AddScoped<ICategoryDal, EfCoreCategoryDal>();

            // IProductService istenirse ProductManager gonderilecek
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();

            services.AddTransient<IEmailSender, EmailSender>();

        }

    
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                SeedDatabase.Seed();
            }

            // root klasorunu disariya acar
            app.UseStaticFiles();

            // extra klasorleri disari acmak icin
            app.CustomStaticFiles();

          
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(routes =>
            {
               routes.MapControllerRoute(
                  name: "adminProducts",
                  pattern: "admin/products",
                  defaults: new { controller = "Admin", action = "ProductList" }
               );

                routes.MapControllerRoute(
                   name: "adminProducts",
                   pattern: "admin/products/{id?}",
                   defaults: new { controller = "Admin", action = "ProductEdit" }
                );  

                routes.MapControllerRoute(
                   name: "products",
                   pattern: "products/{category?}",
                   defaults: new {controller = "Shop", action = "List"}
                );

                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            SeedIdentity.Seed(userManager, roleManager, Configuration).Wait();

        }
    }
}
