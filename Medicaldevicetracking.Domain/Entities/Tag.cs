// MedicalDeviceTracking.Domain/Entities/Tag.cs
using System.ComponentModel.DataAnnotations;
using MedicalDeviceTracking.Domain.Enums;

namespace MedicalDeviceTracking.Domain.Entities;
public class Tag
{
    public Guid Id { get; set; }

    [MaxLength(36)]
    public string? Uuid { get; set; }

    [Required, MaxLength(17)]
    public string MacAddress { get; set; } = string.Empty; // "20:18:ab:cd:20:21"

    public TagType TagType { get; set; }  // mb/ib/ht
    public DeviceType? DeviceType { get; set; } // optional device classification
    [MaxLength(100)]
    public string? AssignedTo { get; set; }
    public TagStatus Status { get; set; } = TagStatus.Active;

    // RSSI threshold rule (ignore readings with rssi > threshold)
    public int? RssiThreshold { get; set; } // e.g., -70

    // Current mapping
    public Guid? CurrentGatewayId { get; set; }
    public int? LastRssi { get; set; }
    public DateTime? LastSeenAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual Gateway? CurrentGateway { get; set; }
    public virtual ICollection<SensorAdvertisement> Advertisements { get; set; } = new List<SensorAdvertisement>();
}
