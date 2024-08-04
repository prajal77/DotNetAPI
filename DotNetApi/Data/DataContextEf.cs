using DotNetApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetApi.Data
{
    public class DataContextEf : DbContext
    {
        private readonly IConfiguration _config;

        public DataContextEf(IConfiguration config)
        {
            _config = config; 
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSalary> UserSalary { get; set; }
        public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure());
            } 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //to find the schema
            modelBuilder.HasDefaultSchema("TutorialAppSchema");
            
            //to map the User model to Users table and Users id
            modelBuilder.Entity<User>().ToTable("Users","TutorialAppSchema")
                .HasKey(u => u.UserId);
            modelBuilder.Entity<UserSalary>().ToTable("UserSalary", "TutorialAppSchema")
                .HasKey(u => u.UserId);
            modelBuilder.Entity<UserJobInfo>().ToTable("UserJobInfo", "TutorialAppSchema")
                .HasKey(u => u.UserId);
        }
    }
}
