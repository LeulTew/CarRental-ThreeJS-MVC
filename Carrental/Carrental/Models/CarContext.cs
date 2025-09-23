using Carrental.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Carrental.Models
{
    public class CarContext : IdentityDbContext<AppUser>
    {
        public CarContext(DbContextOptions<CarContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<CarVersion> CarVersions { get; set; } // Add DbSet for CarVersion
        public DbSet<EmailConfiguration> EmailConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmailConfiguration>().HasNoKey();
            // Configure Booking entity
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasColumnType("decimal(18, 2)");

            // Configure Car entity
            modelBuilder.Entity<Car>()
                .Property(c => c.RentPricePerHour)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Car>()
                .Property(c => c.SalePrice)
                .HasColumnType("decimal(18, 2)");

            // Configure CarVersion entity
            modelBuilder.Entity<CarVersion>()
                .Property(cv => cv.Price)
                .HasColumnType("decimal(18, 2)"); // Specify SQL Server column type for Price

            // Configure Payment entity
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18, 2)");

            // Configure many-to-many relationship between AppUser and Car through Favorite
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.UserId, f.CarId });

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Car)
                .WithMany(c => c.Favorites)
                .HasForeignKey(f => f.CarId);
        }
    }

}
