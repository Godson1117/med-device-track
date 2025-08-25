using System.ComponentModel.DataAnnotations;
using MedicalDeviceTracking.Domain.Enums;

namespace MedicalDeviceTracking.Domain.Entities;

public class SensorAdvertisement
{
    public Guid Id { get; set; }
    public Guid GatewayId { get; set; }
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty;
    [MaxLength(50)]
    public string MacAddress { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int? Rssi { get; set; }
    public int? Battery { get; set; }
    public int? Major { get; set; }
    public int? Minor { get; set; }
    [MaxLength(100)]
    public string? Name { get; set; }
    [MaxLength(100)]
    public string? Uuid { get; set; }
    public int? RssiAtXm { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Humidity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual Gateway Gateway { get; set; } = null!;
}
