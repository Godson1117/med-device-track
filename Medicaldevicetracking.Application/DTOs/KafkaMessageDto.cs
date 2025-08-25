using System.Text.Json.Serialization;

namespace MedicalDeviceTracking.Application.DTOs;

public class KafkaMessageDto
{
    [JsonPropertyName("gw")]
    public string Gateway { get; set; } = string.Empty;

    [JsonPropertyName("tm")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("adv")]
    public List<AdvertisementDto> Advertisements { get; set; } = new();
}

public class AdvertisementDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("mac")]
    public string Mac { get; set; } = string.Empty;

    [JsonPropertyName("tm")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("rssi")]
    public int? Rssi { get; set; }

    [JsonPropertyName("battery")]
    public int? Battery { get; set; }

    [JsonPropertyName("major")]
    public int? Major { get; set; }

    [JsonPropertyName("minor")]
    public int? Minor { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }

    [JsonPropertyName("rssi_at_xm")]
    public int? RssiAtXm { get; set; }

    [JsonPropertyName("temperature")]
    public decimal? Temperature { get; set; }

    [JsonPropertyName("humidity")]
    public decimal? Humidity { get; set; }
}
