using System;
using Microsoft.EntityFrameworkCore;

namespace soc_net.Models
{
    public class SocNetContext : DbContext
	{
        public SocNetContext(DbContextOptions<SocNetContext> options)
            : base(options)
        {
        }

        public SocNetContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity properties and relationships here
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("YourConnectionString");
            }
        }
    }
}

