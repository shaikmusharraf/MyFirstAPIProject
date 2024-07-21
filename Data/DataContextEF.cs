using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{

    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _config;
        public  DataContextEF(IConfiguration config)
        {
            _config=config;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserJobInfo> UserJobInfo { get; set; }
        public DbSet<UserSalary> UserSalary { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            if(!dbContextOptionsBuilder.IsConfigured)
            {
                dbContextOptionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"), dbContextOptionsBuilder => dbContextOptionsBuilder.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TutorialAppSchema");

            modelBuilder.Entity<User>()
            .ToTable("Users","TutorialAppSchema")
            .HasKey(o=> o.UserId);

            modelBuilder.Entity<UserJobInfo>()
            .HasKey(o=> o.UserId);

            modelBuilder.Entity<UserSalary>()
            .HasKey(o=> o.UserId);
        }
    }
}
