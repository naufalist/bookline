using Bookline.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookline.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<AppointmentDayConfig> AppointmentDayConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}