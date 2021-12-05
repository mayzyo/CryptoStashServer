using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try {
                var host = CreateHostBuilder(args).Build();

                if (Environment.GetEnvironmentVariable("MIGRATE") != null)
                {
                    Console.WriteLine("Attempt to populate database with migrations...");
                    Migrate.EnsureMigration(host.Services.GetRequiredService<IConfiguration>());
                }

                Console.WriteLine("Starting host...");
                host.Run();
            } catch (Exception err) {
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
