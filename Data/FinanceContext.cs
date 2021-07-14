using CryptoStashStats.Models;
using CryptoStashStats.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Data
{
    public class FinanceContext : BaseContext
    {
        public DbSet<Coin> Coin { get; set; }
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<Provider> Provider { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<AccountBalance> AccountBalance { get; set; }

        public FinanceContext(DbContextOptions<FinanceContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Coin>()
                .HasIndex(e => e.Ticker)
                .IsUnique();

            builder.Entity<Coin>()
                .HasIndex(e => e.Name)
                .IsUnique();

            builder.Entity<Wallet>()
                .HasIndex(e => e.Address)
                .IsUnique();

            builder.Entity<Provider>()
                .HasIndex(e => e.Name)
                .IsUnique();

            builder.Entity<AccountBalance>()
                .HasIndex(e => new { e.AccountId, e.CoinId })
                .IsUnique();
        }
    }
}
