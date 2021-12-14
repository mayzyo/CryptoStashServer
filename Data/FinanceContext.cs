using CryptoStashStats.Models;
using CryptoStashStats.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Data
{
    // Database access in the context of mining. Anything related to mining rig and mining pools should be accessed here.
    public class FinanceContext : BaseContext, ICurrencyContext
    {
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Blockchain> Blockchains { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletBalance> WalletBalances { get; set; }
        public DbSet<CurrencyExchange> CurrencyExchanges { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<ExchangeAccount> ExchangeAccounts { get; set; }
        public DbSet<ExchangeAccountApiKey> ExchangeAccountApiKeys { get; set; }
        public DbSet<ExchangeAccountBalance> ExchangeAccountBalances { get; set; }

        public FinanceContext(DbContextOptions<FinanceContext> options) : base(options)
        {
  
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("financeSchema");

            builder.Entity<Blockchain>()
                .HasOne(e => e.NativeCurrency)
                .WithOne(e => e.NativeBlockchain);

            builder.Entity<Currency>()
                .HasIndex(e => new { e.Ticker, e.Name })
                .IsUnique();

            builder.Entity<Blockchain>()
                .HasIndex(e => e.Name)
                .IsUnique();

            builder.Entity<Wallet>()
                .HasIndex(e => new { e.Address, e.BlockchainId })
                .IsUnique();

            builder.Entity<CurrencyExchange>()
                .HasIndex(e => e.Name)
                .IsUnique();

            //builder.Entity<ExchangeAccount>()
            //    .HasIndex(e => new { e.Owner, e.CurrencyExchangeId, e.ExchangeAccountApiKeyId })
            //    .IsUnique();

            builder.Entity<ExchangeAccountApiKey>()
                .HasIndex(e => new { e.PublicKey, e.PrivateKey })
                .IsUnique();

            builder.Entity<Currency>()  // Ignored in this context
                .Ignore(e => e.MiningPools); // Defined in MiningContext
        }
    }
}