using CryptoStashStats.Models;
using CryptoStashStats.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Data
{
    // Database access in the context of registered user. This context mainly deals with user related data. Don't mix metrics data in here because user metrics is not always needed when general user info is accessed.
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
