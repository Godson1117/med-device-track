using MedicalDeviceTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalDeviceTracking.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Gateway> Gateways { get; set; }
    public DbSet<SensorAdvertisement> SensorAdvertisements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Gateway configuration
        modelBuilder.Entity<Gateway>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.GatewayId).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.GatewayId).IsUnique();

            // PostgreSQL-compatible default values
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // SensorAdvertisement configuration  
        modelBuilder.Entity<SensorAdvertisement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(20);
            entity.Property(e => e.MacAddress).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Uuid).HasMaxLength(100);

            // PostgreSQL numeric precision
            entity.Property(e => e.Temperature).HasPrecision(5, 2);
            entity.Property(e => e.Humidity).HasPrecision(5, 2);

            // PostgreSQL-compatible default values
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key relationship
            entity.HasOne(e => e.Gateway)
                  .WithMany(g => g.Advertisements)
                  .HasForeignKey(e => e.GatewayId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Indexes for medical device tracking
            entity.HasIndex(e => e.MacAddress);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Timestamp);
        });
    }
}
