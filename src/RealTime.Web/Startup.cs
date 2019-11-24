namespace RealTime.Web
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using System.IO;

    using RealTime.BL;
    using RealTime.BL.Common;
    using RealTime.DAL;
    using Microsoft.Extensions.Options;

    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddAntiforgery(opts => { opts.HeaderName = "X-XSRF-Token"; });
            services.Configure<AppSettings>(this.Configuration.GetSection("AppSettings"));

            var sqlFilePath = Path.Combine(
                Path.GetDirectoryName(WebHostEnvironment.ContentRootPath),
                "prices.db");
            services.AddSingleton(x => x.GetService<IOptions<AppSettings>>().Value);
            services.AddDbContext<PricesDbContext>(
                (options)=> 
                {
                    options.UseSqlite($"Data Source={sqlFilePath};");
                });

            services.RegisterBusinessLogicServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
