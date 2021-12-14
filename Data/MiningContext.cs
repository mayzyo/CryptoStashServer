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
    public class MiningContext : BaseContext, ICurrencyContext
    {
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<MiningPool> MiningPools { get; set; }
        public DbSet<MiningAccount> MiningAccounts { get; set; }
        public DbSet<MiningAccountBalance> MiningAccountBalances { get; set; }
        //public DbSet<MiningAccountWithdrawal> MiningAccountWithdrawals { get; set; } // No controller
        public DbSet<MiningWorker> MiningWorkers { get; set; }
        public DbSet<MiningWorkerHashRate> MiningWorkerHashRates { get; set; }

        public MiningContext(DbContextOptions<MiningContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("miningSchema");

            builder.Entity<Currency>()
                .HasIndex(e => new { e.Ticker, e.Name })
                .IsUnique();

            builder.Entity<MiningPool>()
                .HasIndex(e => e.Name)
                .IsUnique();

            builder.Entity<MiningAccount>()
                .HasIndex(e => new { e.Identifier, e.Owner, e.MiningPoolId })
                .IsUnique();

            builder.Entity<MiningWorker>()
                .HasIndex(e => new { e.Name, e.MiningAccountId })
                .IsUnique();

            //builder.Entity<MiningAccountWithdrawal>()
            //    .HasIndex(e => e.TXHash)
            //    .IsUnique();

            builder.Entity<Currency>()  // Ignored in this context
                .Ignore(e => e.NativeBlockchain) // Define in FinanceContext
                .Ignore(e => e.Blockchains) // Defined in FinanceContext
                .Ignore(e => e.ExchangeAccounts) // Defined in FinanceContext
                .Ignore(e => e.Wallets); // Defined in FinanceContext
        }
    }
}