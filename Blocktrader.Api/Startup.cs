using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.Blocktrader.Service.Files;

namespace Nexus.Blocktrader.Api
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
            services.AddMvc(options => options.EnableEndpointRouting = false).AddJsonOptions(options =>
            {
                // Use the default property (Pascal) casing.
                options.JsonSerializerOptions.PropertyNamingPolicy = null;

                // Configure a custom converter.
                options.JsonSerializerOptions.Converters.Add(new TickerJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new ExchangeTitleJsonConverter());
            });
            
            services.AddLogging(l => l.AddConsole().AddDebug());
            
            services.AddSingleton<ILogger>(sp => LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger<Startup>());
            services.AddSingleton<ITimestampManager>(sp => new BufferedTimestampManager(new FileTimestampManager(sp.GetRequiredService<ILogger>())));

            // In production, the React files will be served from this directory
            //services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseMvc();

            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}