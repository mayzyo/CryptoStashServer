using CryptoStashServer.Models;
using CryptoStashServer.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer.Data
{
    // Database access in the context of mining. Anything related to mining rig and mining pools should be accessed here.
    public class FinanceContext : BaseContext, ITokenContext
    {
        public DbSet<Token> Tokens { get; set; }
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
                .HasOne(e => e.NativeToken)
                .WithOne(e => e.NativeBlockchain);                

            //builder.Entity<Blockchain>()
            //    .HasMany(e => e.Tokens)
            //    .WithMany(e => e.Blockchains);

            builder.Entity<Token>()
                .HasIndex(e => new { e.Name, e.Ticker, e.Address })
                .IsUnique();

            builder.Entity<Blockchain>()
                .HasIndex(e => e.Name)
                .IsUnique();

            builder.Entity<Blockchain>()
                .HasIndex(e => e.NativeTokenId)
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

            builder.Entity<Token>()  // Ignored in this context
                .Ignore(e => e.MiningPools); // Defined in MiningContext
        }
    }
}