using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using NetworkMonitor.Api.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading;
using NetworkMonitor.Utils;
using NetworkMonitor.Objects.Factory;
using HostInitActions;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;
using NetworkMonitor.Utils.Helpers;

namespace NetworkMonitor.Api
{
    public class Startup
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        public Startup(IConfiguration configuration)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private IServiceCollection _services;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _services = services;
              services.AddLogging(builder =>
                {
                    builder.AddConsole();
                });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<ISystemParamsHelper, SystemParamsHelper>();
            services.AddSingleton(_cancellationTokenSource);

            services.Configure<HostOptions>(s => s.ShutdownTimeout = TimeSpan.FromMinutes(5));

            services.AddControllers();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Service Check",
                    Description = "Check if services are running and how fast they are responding. You can check Website, Email, Domain Lookup and Ping.",
                    TermsOfService = new Uri("https://freenetworkmonitor.click/privacypolicy.html"),
                    Contact = new OpenApiContact
                    {
                        Name = "Mahadeva Cottrell",
                        Email = "support@freenetworkmonitor.click",
                        Url = new Uri("https://freenetworkmonitor.click/faq"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Free Network Monitor License",
                        Url = new Uri("https://freenetworkmonitor.click/license.html"),
                    }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStopping.Register(() =>
            {
                _cancellationTokenSource.Cancel();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {

                app.UseHttpsRedirection();

            }
            app.UseCors("AllowAnyOrigin");
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".yaml"] = "text/yaml";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
        }
    }
}
