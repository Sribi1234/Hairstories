using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Hairstories.Models;

namespace Hairstories.Data
{
    /// <summary>
    /// Application database context with Entity Framework Core
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Staff> Staff { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<StaffService> StaffServices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Staff table
            modelBuilder.Entity<Staff>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Staff>()
                .Property(s => s.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Staff>()
                .HasOne(s => s.ApplicationUser)
                .WithOne(u => u.Staff)
                .HasForeignKey<Staff>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Customer table
            modelBuilder.Entity<Customer>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Appointments)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Service table
            modelBuilder.Entity<Service>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            // Configure Appointment table
            modelBuilder.Entity<Appointment>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Appointment>()
                .Property(a => a.PaymentStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Staff)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure StaffService junction table
            modelBuilder.Entity<StaffService>()
                .HasKey(ss => ss.Id);

            modelBuilder.Entity<StaffService>()
                .HasOne(ss => ss.Staff)
                .WithMany(s => s.StaffServices)
                .HasForeignKey(ss => ss.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StaffService>()
                .HasOne(ss => ss.Service)
                .WithMany(s => s.StaffServices)
                .HasForeignKey(ss => ss.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes for better query performance
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.StaffId, a.AppointmentDateTime });

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.CustomerId);

            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email);

            modelBuilder.Entity<Service>()
                .HasIndex(s => s.ServiceName);
        }
    }
}
