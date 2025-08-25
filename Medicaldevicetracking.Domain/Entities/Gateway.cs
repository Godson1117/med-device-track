using System.ComponentModel.DataAnnotations;

namespace MedicalDeviceTracking.Domain.Entities;

public class Gateway
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string GatewayId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<SensorAdvertisement> Advertisements { get; set; } = new List<SensorAdvertisement>();
}
