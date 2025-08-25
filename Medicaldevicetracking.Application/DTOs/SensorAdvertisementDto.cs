namespace MedicalDeviceTracking.Application.DTOs;

public class SensorAdvertisementDto
{
    public Guid Id { get; set; }
    public Guid GatewayId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int? Rssi { get; set; }
    public int? Battery { get; set; }
    public int? Major { get; set; }
    public int? Minor { get; set; }
    public string? Name { get; set; }
    public string? Uuid { get; set; }
    public int? RssiAtXm { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Humidity { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateSensorAdvertisementDto
{
    public string Type { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int? Rssi { get; set; }
    public int? Battery { get; set; }
    public int? Major { get; set; }
    public int? Minor { get; set; }
    public string? Name { get; set; }
    public string? Uuid { get; set; }
    public int? RssiAtXm { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Humidity { get; set; }
}
