using CryptoStashStats.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats
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
            // Setup Entity Core connection to PostgreSQL.
            NpgsqlConnectionStringBuilder builder;
            if (Environment.GetEnvironmentVariable("PGSQLCONNSTR_CryptoDb") != null)
            {
                // Get connection string from environment variable.
                builder = new NpgsqlConnectionStringBuilder(Environment.GetEnvironmentVariable("PGSQLCONNSTR_CryptoDb"));
            } else
            {
                // Get connection string from user secrets.
                builder = new NpgsqlConnectionStringBuilder(Configuration.GetConnectionString("CryptoDb"));
                if (Configuration["CryptoDb"] != null) builder.Password = Configuration["CryptoDb"];
            }

            services.AddDbContext<UserContext>(options => options.UseNpgsql(builder.ConnectionString));
            services.AddDbContext<MinerContext>(options => options.UseNpgsql(builder.ConnectionString));
            services.AddDbContext<FinanceContext>(options => options.UseNpgsql(builder.ConnectionString));

            // Setup JSON loop handling.
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            // Generate OpenAPI JSON file.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CryptoStashStats", Version = "v3" });
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CryptoStashStats v1"));
            }

            // Setup CORS policy based on environment variable.
            var origins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS") ?? "*";
            // CORS setting with CorsPolicyBuilder.
            app.UseCors(builder =>
            {
                builder
                .WithOrigins(origins)
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
