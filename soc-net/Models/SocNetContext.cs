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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PostLike>()
               .HasKey(pl => new { pl.PostId, pl.UserId });
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

