using Microsoft.EntityFrameworkCore;
using UserApi.DataLayer.Entity;

namespace UserApi.DataLayer
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserLogHistory> HistoryVersions { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new SeedData(modelBuilder).Seed();
        }
    }
}
