using LeaveManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<LeaveLimit> LeaveLimits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed default values for LeaveLimits table
            modelBuilder.Entity<LeaveLimit>().HasData(
                new LeaveLimit { Id = 1, LeaveType = "Paid", MaxDays = 3 },
                new LeaveLimit { Id = 2, LeaveType = "Sick", MaxDays = 3 },
                new LeaveLimit { Id = 3, LeaveType = "Vacation", MaxDays = 3 }
            );
        }

    }
}