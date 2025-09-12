// DTOs/TagCrudDtos.cs
using System.ComponentModel.DataAnnotations;
using MedicalDeviceTracking.Domain.Enums;

namespace MedicalDeviceTracking.Application.DTOs;
public class TagReadDto
{
    public Guid Id { get; set; }
    public string? Uuid { get; set; }
    public string MacAddress { get; set; } = string.Empty;
    public TagType TagType { get; set; }
    public DeviceType? DeviceType { get; set; }
    public string? AssignedTo { get; set; }
    public TagStatus Status { get; set; }
    public int? RssiThreshold { get; set; }
    public Guid? CurrentGatewayId { get; set; }
    public int? LastRssi { get; set; }
    public DateTime? LastSeenAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TagCreateDto
{
    [MaxLength(36)]
    public string? Uuid { get; set; }

    [Required, MaxLength(17)]
    public string MacAddress { get; set; } = string.Empty;

    [Required]
    public TagType TagType { get; set; }

    public DeviceType? DeviceType { get; set; }
    [MaxLength(100)]
    public string? AssignedTo { get; set; }
    public TagStatus Status { get; set; } = TagStatus.Active;

    // e.g., -70 means ignore readings > -70
    public int? RssiThreshold { get; set; }
}

public class TagUpdateDto : TagCreateDto { }
