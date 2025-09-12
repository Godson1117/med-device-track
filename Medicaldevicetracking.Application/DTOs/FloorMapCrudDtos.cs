// DTOs/FloorMapCrudDtos.cs
using System.ComponentModel.DataAnnotations;

namespace MedicalDeviceTracking.Application.DTOs;
public class FloorMapReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FloorMapCreateDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required, MaxLength(500)]
    public string ImagePath { get; set; } = string.Empty;

    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public bool IsActive { get; set; } = true;
}

public class FloorMapUpdateDto : FloorMapCreateDto { }
