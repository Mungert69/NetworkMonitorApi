using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.StaticFiles;

using NetworkMonitor.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading;
using NetworkMonitor.Utils;
using NetworkMonitor.Auth;
using NetworkMonitor.Objects.Factory;
using HostInitActions;
using Microsoft.Extensions.FileProviders;

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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
            services.AddSingleton<INetLoggerFactory, NetLoggerFactory>();
            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton(_cancellationTokenSource);

            services.Configure<HostOptions>(s => s.ShutdownTimeout = TimeSpan.FromMinutes(5));

            services.AddControllers();
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
