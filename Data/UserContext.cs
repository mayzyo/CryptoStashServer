using CryptoStashStats.Models;
using CryptoStashStats.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Data
{
    public class UserContext : BaseContext
    {
        public DbSet<User> User { get; set; }

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(e => e.Username)
                .IsUnique();
        }
    }
}
