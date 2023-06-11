using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MetroLog;
using NetworkMonitor.Objects.Factory;
using System;
using System.Net;
namespace NetworkMonitor.Api
{
    public class Program
    {
        //private bool _isDevelopmentMode;
        public static void Main(string[] args)
        {
            bool isDevelopmentMode = false;
            string appFile = "appsettings.json";
            for (int i = 0; i < args.Length; i++) // Loop through array
            {
                Console.WriteLine("ARGS are : " + args[i]);
                isDevelopmentMode = true;
                appFile = "appsettings.json";
            }
            IConfigurationRoot config = new ConfigurationBuilder()
        .AddJsonFile(appFile, optional: false)
        .Build();
            IWebHost host = CreateWebHostBuilder(isDevelopmentMode).Build();
            host.Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(bool isDevelopmentMode) =>
        WebHost.CreateDefaultBuilder().UseKestrel(options =>
    {
        //options.Listen(IPAddress.Loopback, 5000);  // http:localhost:5000
        if (isDevelopmentMode)
        {
            options.Listen(IPAddress.Any, 2086);         // http:*:65123
            options.Listen(IPAddress.Any, 2087, listenOptions =>
            {
                listenOptions.UseHttps("https.pfx", "Ac£0462110");
            });
        }
        else
        {
            options.Listen(IPAddress.Any, 2086);         // http:*:65123
            options.Listen(IPAddress.Any, 2087, listenOptions =>
            {
                listenOptions.UseHttps("https.pfx", "Ac£0462110");
            });
        }
    }).UseStartup<Startup>();
    }
}
