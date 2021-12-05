using CryptoStashStats.Data;
using CryptoStashStats.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
            // Setup OAuth2 JWT.
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Environment.GetEnvironmentVariable("AUTHORITY_URL");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudiences = new[]
                        {
                            "urn:finance",
                            "urn:mining"
                        },
                        ValidateAudience = true
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("finance_audience", policy => policy.RequireClaim("aud", "urn:finance"));
                options.AddPolicy("mining_audience", policy => policy.RequireClaim("aud", "urn:mining"));
                options.AddPolicy("manage_access", policy => policy.RequireClaim("scope", "manage"));
                options.AddPolicy("read_access", policy => policy.RequireClaim("scope", "finance.read", "mining.read"));
                options.AddPolicy("write_access", policy => policy.RequireClaim("scope", "finance.write", "mining.write"));
            });

            // Setup custom services.
            services.AddSingleton<IPasswordHelper, PasswordHelper>();

            // Setup Entity Core connection to PostgreSQL.
            NpgsqlConnectionStringBuilder connBuilder = new NpgsqlConnectionStringBuilder(Configuration.GetConnectionString("StatsDb"));
            // Get connection string from user secrets.
            if (Configuration["StatsDb"] != null) connBuilder.Password = Configuration["StatsDb"];
            services.AddDbContext<FinanceContext>(options => options.UseNpgsql(
                connBuilder.ConnectionString,
                x => x.MigrationsHistoryTable("__FinanceMigrationsHistory", "financeSchema")
            ));
            services.AddDbContext<MiningContext>(options => options.UseNpgsql(
                connBuilder.ConnectionString,
                x => x.MigrationsHistoryTable("__MiningMigrationsHistory", "miningSchema")
            ));

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
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
