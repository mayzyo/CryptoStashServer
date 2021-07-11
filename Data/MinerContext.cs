using CryptoStashStats.Models;
using CryptoStashStats.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Data
{
    public class MinerContext : BaseContext
    {
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<Worker> Worker { get; set; }
        public DbSet<Hashrate> Hashrate { get; set; }
        public DbSet<PoolBalance> PoolBalance { get; set; }
        public DbSet<MiningPool> MiningPool { get; set; }
        public DbSet<Payout> Payout { get; set; }

        public MinerContext(DbContextOptions<MinerContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Coin>()
                .HasIndex(el => el.Ticker)
                .IsUnique();

            builder.Entity<Coin>()
                .HasIndex(el => el.Name)
                .IsUnique();

            builder.Entity<Wallet>()
                .HasIndex(el => el.Address)
                .IsUnique();

            builder.Entity<Worker>()
                .HasIndex(el => el.Name)
                .IsUnique();

            builder.Entity<PoolBalance>()
                .HasIndex(el => new { el.MiningPoolId, el.WalletId })
                .IsUnique();

            builder.Entity<MiningPool>()
                .HasIndex(el => el.Name)
                .IsUnique();

            builder.Entity<Payout>()
                .HasIndex(el => el.TXHash)
                .IsUnique();

            builder.Entity<Payout>()
                .HasIndex(el => new { el.MiningPoolId, el.WalletId })
                .IsUnique();
        }
    }
}
