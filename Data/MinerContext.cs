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
            builder.Entity<Worker>()
                .HasIndex(e => new { e.Name, e.Address })
                .IsUnique();

            builder.Entity<PoolBalance>()
                .HasIndex(e => new { e.MiningPoolId, e.Address })
                .IsUnique();

            builder.Entity<MiningPool>()
                .HasIndex(e => e.Name)
                .IsUnique();

            builder.Entity<Payout>()
                .HasIndex(e => e.TXHash)
                .IsUnique();
        }
    }
}

/*
 * Clear context tables command:
 * TRUNCATE TABLE "Hashrate" CASCADE;TRUNCATE TABLE "MiningPool" CASCADE;TRUNCATE TABLE "Payout" CASCADE;TRUNCATE TABLE "PoolBalance" CASCADE;TRUNCATE TABLE "Worker" CASCADE;
 */