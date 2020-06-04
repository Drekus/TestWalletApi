using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestWalletApi.Data;
using TestWalletApi.Domain.Converting;
using TestWalletApi.Services;

namespace TestWalletApi
{
    public class Startup
    {
        private string _contentRootPath;
        

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            _contentRootPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("AttachConnection"); 
            if (connectionString.Contains("%CONTENT_ROOT_PATH%"))
                connectionString = connectionString.Replace("%CONTENT_ROOT_PATH%", _contentRootPath);

            services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(connectionString));

            services.AddControllers();

            services.AddHttpClient();

            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IIdempotentService, IdempotentService>();
            services.AddScoped<ICurrencyRatesGetter, CurrencyRatesFromEuroBankGetter>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
