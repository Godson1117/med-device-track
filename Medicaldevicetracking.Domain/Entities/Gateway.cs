// MedicalDeviceTracking.Domain/Entities/Gateway.cs
using System.ComponentModel.DataAnnotations;
using MedicalDeviceTracking.Domain.Enums;

namespace MedicalDeviceTracking.Domain.Entities;
public class Gateway
{
    public Guid Id { get; set; }

    [Required, MaxLength(50)]
    public string GatewayId { get; set; } = string.Empty; // From Kafka "gw"

    [MaxLength(36)]
    public string? Uuid { get; set; }

    [MaxLength(17)]
    public string? MacAddress { get; set; } // "ac:23:3f:c0:4c:e9"

    [MaxLength(100)]
    public string? GatewayName { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    public Guid? FloorMapId { get; set; }

    // Coordinates on the floor map
    public decimal? CoordinatesX { get; set; } // DECIMAL(10,6)
    public decimal? CoordinatesY { get; set; } // DECIMAL(10,6)

    public GatewayStatus Status { get; set; } = GatewayStatus.Active;

    public DateTime Timestamp { get; set; }      // Last telemetry ts (UTC)
    public DateTime CreatedAt { get; set; }      // DB default
    public DateTime UpdatedAt { get; set; }      // DB default

    public virtual FloorMap? FloorMap { get; set; }
    public virtual ICollection<SensorAdvertisement> Advertisements { get; set; } = new List<SensorAdvertisement>();
    public virtual ICollection<Tag> TagsCurrentlyMapped { get; set; } = new List<Tag>();
}
