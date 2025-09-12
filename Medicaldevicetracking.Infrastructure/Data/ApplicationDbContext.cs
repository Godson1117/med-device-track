// MedicalDeviceTracking.Infrastructure/Data/ApplicationDbContext.cs (excerpt)
using MedicalDeviceTracking.Domain.Entities;
using MedicalDeviceTracking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MedicalDeviceTracking.Infrastructure.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Gateway> Gateways { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<FloorMap> FloorMaps { get; set; }
    public DbSet<SensorAdvertisement> SensorAdvertisements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Gateway
        modelBuilder.Entity<Gateway>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.GatewayId).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.GatewayId).IsUnique();
            entity.Property(e => e.Uuid).HasMaxLength(36);
            entity.Property(e => e.MacAddress).HasMaxLength(17);
            entity.HasIndex(e => e.MacAddress).IsUnique().HasFilter("\"MacAddress\" IS NOT NULL");
            entity.Property(e => e.GatewayName).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.CoordinatesX).HasPrecision(10, 6);
            entity.Property(e => e.CoordinatesY).HasPrecision(10, 6);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(e => e.FloorMap)
                  .WithMany(f => f.Gateways)
                  .HasForeignKey(e => e.FloorMapId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Uuid).HasMaxLength(36);
            entity.HasIndex(e => e.Uuid);
            entity.Property(e => e.MacAddress).IsRequired().HasMaxLength(17);
            entity.HasIndex(e => e.MacAddress).IsUnique();
            entity.Property(e => e.TagType).HasConversion<string>().HasMaxLength(5);
            entity.Property(e => e.DeviceType).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.AssignedTo).HasMaxLength(100);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(e => e.CurrentGateway)
                  .WithMany(g => g.TagsCurrentlyMapped)
                  .HasForeignKey(e => e.CurrentGatewayId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // FloorMap
        modelBuilder.Entity<FloorMap>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImagePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Width).HasPrecision(10, 2);
            entity.Property(e => e.Height).HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // SensorAdvertisement
        modelBuilder.Entity<SensorAdvertisement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(20);
            entity.Property(e => e.MacAddress).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Uuid).HasMaxLength(100);
            entity.Property(e => e.Temperature).HasPrecision(5, 2);
            entity.Property(e => e.Humidity).HasPrecision(5, 2);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(e => e.Gateway)
                  .WithMany(g => g.Advertisements)
                  .HasForeignKey(e => e.GatewayId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Tag)
                  .WithMany(t => t.Advertisements)
                  .HasForeignKey(e => e.TagId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.MacAddress);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => new { e.GatewayId, e.Timestamp });
            entity.HasIndex(e => new { e.TagId, e.Timestamp });
        });
    }
}
