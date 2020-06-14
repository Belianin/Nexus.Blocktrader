using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Nexus.Logging;
using Nexus.Logging.Console;
using Nexus.Prophecy.DI;
using Nexus.Prophecy.Logs;

namespace Nexus.Prophecy
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
            var settings = JsonConvert.DeserializeObject<ProphecySettings>(File.ReadAllText("prophecy.settings.json"));
            
            services.AddSingleton<ILog, ColourConsoleLog>();
            services.AddSingleton<ILogService>(sp => new LogService(settings.Services));
            services.AddNotificatorService();
            services.AddControllers();
            
            services.AddMvc(options => options.EnableEndpointRouting = false).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Converters.Add(new LogLevelJsonConverter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILog log)
        {
            log.Info("Starting Prophecy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseMvc();
        }
    }
}