using CryptoStashServer.Data;
using CryptoStashServer.Models;
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
    public class SeedData
    {
        public static void EnsureSeedData(IConfiguration config)
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

                    if(financeContext.Tokens.Count() == 0)
                    {
                        var tokens = new List<Token>
                        {
                            new Token { Id = 1000, Name = "BITCOIN", Ticker = "BTC" },
                            new Token { Id = 1001, Name = "ETHER", Ticker = "ETH" },
                            new Token { Id = 1002, Name = "BINANCE COIN", Ticker = "BNB" },
                            new Token { Id = 1003, Name = "TETHER", Ticker = "USDT" },
                            new Token { Id = 1004, Name = "SOLANA", Ticker = "SOL" },
                            new Token { Id = 1005, Name = "CARDANO", Ticker = "ADA" },
                            new Token { Id = 1006, Name = "USD COIN", Ticker = "USDC" },
                            new Token { Id = 1007, Name = "XRP", Ticker = "XRP" },
                            new Token { Id = 1008, Name = "TERRA", Ticker = "LUNA" },
                            new Token { Id = 1009, Name = "POLKADOT", Ticker = "DOT" },
                            new Token { Id = 1010, Name = "AVALANCHE", Ticker = "AVAX" },
                            new Token { Id = 1011, Name = "DOGECOIN", Ticker = "DOGE" },
                            new Token { Id = 1012, Name = "SHIBA INU", Ticker = "SHIB" },
                            new Token { Id = 1013, Name = "POLYGON", Ticker = "MATIC" },
                            new Token { Id = 1014, Name = "BINANCE USD", Ticker = "BUSD" },
                            new Token { Id = 1015, Name = "CRYPTO.COM COIN", Ticker = "CRO" },
                            new Token { Id = 1016, Name = "WRAPPED BITCOIN", Ticker = "WBTC" },
                            new Token { Id = 1017, Name = "UNISWAP", Ticker = "UNI" },
                            new Token { Id = 1018, Name = "LITECOIN", Ticker = "LTC" },
                            new Token { Id = 1019, Name = "TERRA USD", Ticker = "UST" },
                            new Token { Id = 1020, Name = "ALGORAND", Ticker = "ALGO" },
                            new Token { Id = 1021, Name = "ZILLIQA", Ticker = "ZIL" },
                            new Token { Id = 1022, Name = "CHIA NETWORK", Ticker = "XCH" },
                            new Token { Id = 1023, Name = "NERVOS NETWORK", Ticker = "CKB" }
                        };

                        financeContext.AddRange(tokens);
                        miningContext.AddRange(tokens);

                        var blockchains = new List<Blockchain>
                        {
                            new Blockchain { Name = "ETHEREUM", NativeToken = tokens[1] },
                            new Blockchain { Name = "SOLANA", NativeToken = tokens[4] },
                            new Blockchain { Name = "POLYGON", NativeToken = tokens[13] }
                        };

                        financeContext.AddRange(blockchains);

                        var currencyExchanges = new List<CurrencyExchange>
                        {
                            new CurrencyExchange { Name = "BINANCE" }
                        };

                        financeContext.AddRange(currencyExchanges);

                        var miningPools = new List<MiningPool>
                        {
                            new MiningPool { 
                                Name = "EZIL", 
                                Tokens = new List<Token> { tokens[1], tokens[21] } 
                            }
                        };

                        miningContext.AddRange(miningPools);

                        financeContext.SaveChanges();
                        miningContext.SaveChanges();
                    }
                }
            }
        }
    }
}
