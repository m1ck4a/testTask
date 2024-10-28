using Microsoft.EntityFrameworkCore;
using ProjectManagement.Architecture.Entities;

namespace Project_Job.DataAccess
{
    public class ProjectManagementDbContext : DbContext
    {
        public ProjectManagementDbContext()
        {
            Database.EnsureCreated();
        }

        public ProjectManagementDbContext(DbContextOptions<ProjectManagementDbContext> options):
            base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=TestDb.db");
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<UserEntity>()
        //        .HasOne(u => u.Role)
        //        .WithMany()
        //        .HasForeignKey(u => u.RoleId);
        //}

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<ProjectEntity> Projects { get; set; }
    }
}
