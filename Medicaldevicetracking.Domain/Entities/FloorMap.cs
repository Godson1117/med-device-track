// MedicalDeviceTracking.Domain/Entities/FloorMap.cs
using System.ComponentModel.DataAnnotations;

namespace MedicalDeviceTracking.Domain.Entities;
public class FloorMap
{
    public Guid Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required, MaxLength(500)]
    public string ImagePath { get; set; } = string.Empty;

    public decimal Width { get; set; }   // units consistent with coordinates
    public decimal Height { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Gateway> Gateways { get; set; } = new List<Gateway>();
}
