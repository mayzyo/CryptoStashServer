using CryptoStashServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer
{
    public class Migrate
    {
        public static void EnsureMigration(IConfiguration config)
        {
            // Setup Entity Core connection to PostgreSQL.
            NpgsqlConnectionStringBuilder connBuilder = new NpgsqlConnectionStringBuilder(config.GetConnectionString("StatsDb"));
            // Get connection string from user secrets.
            if (config["StatsDb"] != null) connBuilder.Password = config["StatsDb"];

            var services = new ServiceCollection();

            services.AddDbContext<FinanceContext>(options => options.UseNpgsql(
                connBuilder.ConnectionString,
                x => x.MigrationsHistoryTable("__FinanceMigrationsHistory", "financeSchema")
            ));
            services.AddDbContext<MiningContext>(options => options.UseNpgsql(
                connBuilder.ConnectionString,
                x => x.MigrationsHistoryTable("__MiningMigrationsHistory", "miningSchema")
            ));

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var financeContext = scope.ServiceProvider.GetService<FinanceContext>();
                    financeContext.Database.Migrate();
                    var miningContext = scope.ServiceProvider.GetService<MiningContext>();
                    miningContext.Database.Migrate();
                }
            }
        }
    }
}
