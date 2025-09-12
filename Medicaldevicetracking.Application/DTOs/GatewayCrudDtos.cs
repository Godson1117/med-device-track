// DTOs/GatewayCrudDtos.cs
using System.ComponentModel.DataAnnotations;
using MedicalDeviceTracking.Domain.Enums;

namespace MedicalDeviceTracking.Application.DTOs;
public class GatewayReadDto
{
    public Guid Id { get; set; }
    public string GatewayId { get; set; } = string.Empty;
    public string? Uuid { get; set; }
    public string? MacAddress { get; set; }
    public string? GatewayName { get; set; }
    public string? Location { get; set; }
    public Guid? FloorMapId { get; set; }
    public decimal? CoordinatesX { get; set; }
    public decimal? CoordinatesY { get; set; }
    public GatewayStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GatewayCreateDto
{
    [Required, MaxLength(50)]
    public string GatewayId { get; set; } = string.Empty;
    [MaxLength(36)]
    public string? Uuid { get; set; }
    [MaxLength(17)]
    public string? MacAddress { get; set; }
    [MaxLength(100)]
    public string? GatewayName { get; set; }
    [MaxLength(200)]
    public string? Location { get; set; }
    public Guid? FloorMapId { get; set; }
    public decimal? CoordinatesX { get; set; }
    public decimal? CoordinatesY { get; set; }
    public GatewayStatus Status { get; set; } = GatewayStatus.Active;
}

public class GatewayUpdateDto : GatewayCreateDto { }
