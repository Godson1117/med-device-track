namespace MedicalDeviceTracking.Application.DTOs;

public class GatewayDto
{
    public Guid Id { get; set; }
    public string GatewayId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<SensorAdvertisementDto> Advertisements { get; set; } = new();
}

public class CreateGatewayDto
{
    public string GatewayId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public List<CreateSensorAdvertisementDto> Advertisements { get; set; } = new();
}
