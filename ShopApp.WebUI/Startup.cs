using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopApp.Business.Abstract;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Abstract;
using ShopApp.DataAccess.Concrete.EfCore;
using ShopApp.WebUI.Middlewares;

namespace ShopApp.WebUI
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();          

            services.AddScoped<IProductDal, EfCoreProductDal>();
            services.AddScoped<ICategoryDal, EfCoreCategoryDal>();

            // IProductService istenirse ProductManager gonderilecek
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();

        }

    
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

        }
    }
}
