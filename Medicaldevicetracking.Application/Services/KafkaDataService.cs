using AutoMapper;
using MedicalDeviceTracking.Application.DTOs;
using MedicalDeviceTracking.Application.Interfaces;
using MedicalDeviceTracking.Domain.Entities;
using MedicalDeviceTracking.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicalDeviceTracking.Application.Services;

public class KafkaDataService : IKafkaDataService
{
    private readonly IGatewayRepository _gatewayRepository;
    private readonly ISensorAdvertisementRepository _sensorRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<KafkaDataService> _logger;

    public KafkaDataService(
        IGatewayRepository gatewayRepository,
        ISensorAdvertisementRepository sensorRepository,
        ITagRepository tagRepository,
        IMapper mapper,
        ILogger<KafkaDataService> logger)
    {
        _gatewayRepository = gatewayRepository;
        _sensorRepository = sensorRepository;
        _tagRepository = tagRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<GatewayDto>> ProcessKafkaMessageAsync(KafkaMessageDto message)
    {
        try
        {
            _logger.LogInformation("Processing Kafka message for medical device gateway: {GatewayId}", message.Gateway);

            // Parse gateway timestamp (normalized to UTC)
            DateTime gatewayTimestamp;
            if (DateTime.TryParse(message.Timestamp, out var parsedTimestamp))
            {
                gatewayTimestamp = parsedTimestamp.Kind == DateTimeKind.Utc
                    ? parsedTimestamp
                    : parsedTimestamp.ToUniversalTime();
            }
            else
            {
                gatewayTimestamp = DateTime.UtcNow;
            }

            // Upsert gateway by GatewayId
            var existingGateway = await _gatewayRepository.GetByGatewayIdAsync(message.Gateway);
            Gateway gateway;
            if (existingGateway == null)
            {
                gateway = new Gateway
                {
                    Id = Guid.NewGuid(),
                    GatewayId = message.Gateway,
                    Timestamp = gatewayTimestamp,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                gateway = await _gatewayRepository.AddAsync(gateway);
            }
            else
            {
                gateway = existingGateway;
                gateway.Timestamp = gatewayTimestamp;
                gateway.UpdatedAt = DateTime.UtcNow;
                gateway = await _gatewayRepository.UpdateAsync(gateway);
            }

            // Process advertisements (resolve Tag, apply RSSI rules, persist with TagId)
            var advertisements = new List<SensorAdvertisement>();

            foreach (var adv in message.Advertisements)
            {
                // Parse adv timestamp (UTC)
                DateTime advTimestamp;
                if (DateTime.TryParse(adv.Timestamp, out var parsedAdvTimestamp))
                {
                    advTimestamp = parsedAdvTimestamp.Kind == DateTimeKind.Utc
                        ? parsedAdvTimestamp
                        : parsedAdvTimestamp.ToUniversalTime();
                }
                else
                {
                    advTimestamp = DateTime.UtcNow;
                }

                // Resolve or create Tag by MAC if MAC present
                Tag? tag = null;
                if (!string.IsNullOrWhiteSpace(adv.Mac))
                {
                    // Normalize MAC to colon form if needed (store as-is if normalization is elsewhere)
                    var mac = adv.Mac;

                    tag = await _tagRepository.GetByMacAsync(mac);
                    if (tag == null)
                    {
                        tag = new Tag
                        {
                            Id = Guid.NewGuid(),
                            MacAddress = mac,
                            Uuid = adv.Uuid,
                            // Defaults: status active, optional threshold; adjust if creation rules exist elsewhere
                            Status = Domain.Enums.TagStatus.Active,
                            RssiThreshold = tag?.RssiThreshold, // remains null unless defaulting
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        await _tagRepository.AddAsync(tag);
                    }
                    else if (string.IsNullOrWhiteSpace(tag.Uuid) && !string.IsNullOrWhiteSpace(adv.Uuid))
                    {
                        tag.Uuid = adv.Uuid;
                        await _tagRepository.UpdateAsync(tag);
                    }

                    // Apply RSSI threshold mapping rule: ignore if stronger than threshold (numerically greater)
                    // Example: threshold -70 => ignore -60 (since -60 > -70)
                    var considerForMapping = adv.Rssi.HasValue &&
                                             (!tag.RssiThreshold.HasValue || adv.Rssi.Value <= tag.RssiThreshold.Value);

                    // Less RSSI (more negative) wins among gateways, or map if no current mapping
                    if (considerForMapping)
                    {
                        var shouldRemap =
                            !tag.LastRssi.HasValue ||
                            !tag.CurrentGatewayId.HasValue ||
                            adv.Rssi.Value < tag.LastRssi.Value;

                        if (shouldRemap)
                        {
                            tag.CurrentGatewayId = gateway.Id;
                            tag.LastRssi = adv.Rssi;
                            tag.LastSeenAt = advTimestamp;
                            await _tagRepository.UpdateAsync(tag);
                        }
                    }
                }

                var sensorAdv = new SensorAdvertisement
                {
                    Id = Guid.NewGuid(),
                    GatewayId = gateway.Id,
                    TagId = tag?.Id,
                    Type = adv.Type,
                    MacAddress = adv.Mac,
                    Timestamp = advTimestamp,
                    Rssi = adv.Rssi,
                    Battery = adv.Battery,
                    Major = adv.Major,
                    Minor = adv.Minor,
                    Name = adv.Name,
                    Uuid = adv.Uuid,
                    RssiAtXm = adv.RssiAtXm,
                    Temperature = adv.Temperature,
                    Humidity = adv.Humidity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                advertisements.Add(sensorAdv);
            }

            if (advertisements.Any())
            {
                await _sensorRepository.AddRangeAsync(advertisements);
            }

            // Build response DTO
            var result = new GatewayDto
            {
                Id = gateway.Id,
                GatewayId = gateway.GatewayId,
                Timestamp = gateway.Timestamp,
                CreatedAt = gateway.CreatedAt,
                Advertisements = advertisements.Select(a => new SensorAdvertisementDto
                {
                    Id = a.Id,
                    GatewayId = a.GatewayId,
                    // Note: SensorAdvertisementDto can be extended to include TagId if needed
                    Type = a.Type,
                    MacAddress = a.MacAddress,
                    Timestamp = a.Timestamp,
                    Rssi = a.Rssi,
                    Battery = a.Battery,
                    Major = a.Major,
                    Minor = a.Minor,
                    Name = a.Name,
                    Uuid = a.Uuid,
                    RssiAtXm = a.RssiAtXm,
                    Temperature = a.Temperature,
                    Humidity = a.Humidity,
                    CreatedAt = a.CreatedAt
                }).ToList()
            };

            _logger.LogInformation("Successfully processed medical device data for gateway: {GatewayId} with {Count} advertisements",
                message.Gateway, advertisements.Count);

            return ApiResponse<GatewayDto>.SuccessResponse(result, "Medical device data processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing medical device data for gateway: {GatewayId}", message.Gateway);
            return ApiResponse<GatewayDto>.ErrorResponse("Failed to process medical device data", 500, new List<string> { ex.Message });
        }
    }

    public async Task<ApiResponse<PagedResult<GatewayDto>>> GetGatewaysAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var gateways = await _gatewayRepository.GetAllAsync(pageNumber, pageSize);
            var totalCount = await _gatewayRepository.GetTotalCountAsync();

            var gatewayDtos = gateways.Select(g => new GatewayDto
            {
                Id = g.Id,
                GatewayId = g.GatewayId,
                Timestamp = g.Timestamp,
                CreatedAt = g.CreatedAt,
                Advertisements = g.Advertisements.Select(a => new SensorAdvertisementDto
                {
                    Id = a.Id,
                    GatewayId = a.GatewayId,
                    Type = a.Type,
                    MacAddress = a.MacAddress,
                    Timestamp = a.Timestamp,
                    Rssi = a.Rssi,
                    Battery = a.Battery,
                    Major = a.Major,
                    Minor = a.Minor,
                    Name = a.Name,
                    Uuid = a.Uuid,
                    RssiAtXm = a.RssiAtXm,
                    Temperature = a.Temperature,
                    Humidity = a.Humidity,
                    CreatedAt = a.CreatedAt
                }).ToList()
            });

            var result = new PagedResult<GatewayDto>
            {
                Items = gatewayDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
            return ApiResponse<PagedResult<GatewayDto>>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving medical device gateways");
            return ApiResponse<PagedResult<GatewayDto>>.ErrorResponse("Failed to retrieve medical device gateways", 500);
        }
    }

    public async Task<ApiResponse<GatewayDto?>> GetGatewayByIdAsync(Guid id)
    {
        try
        {
            var gateway = await _gatewayRepository.GetByIdAsync(id);
            if (gateway == null)
            {
                return ApiResponse<GatewayDto?>.ErrorResponse("Medical device gateway not found", 404);
            }

            var result = new GatewayDto
            {
                Id = gateway.Id,
                GatewayId = gateway.GatewayId,
                Timestamp = gateway.Timestamp,
                CreatedAt = gateway.CreatedAt,
                Advertisements = gateway.Advertisements.Select(a => new SensorAdvertisementDto
                {
                    Id = a.Id,
                    GatewayId = a.GatewayId,
                    Type = a.Type,
                    MacAddress = a.MacAddress,
                    Timestamp = a.Timestamp,
                    Rssi = a.Rssi,
                    Battery = a.Battery,
                    Major = a.Major,
                    Minor = a.Minor,
                    Name = a.Name,
                    Uuid = a.Uuid,
                    RssiAtXm = a.RssiAtXm,
                    Temperature = a.Temperature,
                    Humidity = a.Humidity,
                    CreatedAt = a.CreatedAt
                }).ToList()
            };

            return ApiResponse<GatewayDto?>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving medical device gateway: {Id}", id);
            return ApiResponse<GatewayDto?>.ErrorResponse("Failed to retrieve medical device gateway", 500);
        }
    }

    public async Task<ApiResponse<PagedResult<SensorAdvertisementDto>>> GetSensorDataAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var sensors = await _sensorRepository.GetAllAsync(pageNumber, pageSize);
            var totalCount = await _sensorRepository.GetTotalCountAsync();

            var sensorDtos = sensors.Select(s => new SensorAdvertisementDto
            {
                Id = s.Id,
                GatewayId = s.GatewayId,
                Type = s.Type,
                MacAddress = s.MacAddress,
                Timestamp = s.Timestamp,
                Rssi = s.Rssi,
                Battery = s.Battery,
                Major = s.Major,
                Minor = s.Minor,
                Name = s.Name,
                Uuid = s.Uuid,
                RssiAtXm = s.RssiAtXm,
                Temperature = s.Temperature,
                Humidity = s.Humidity,
                CreatedAt = s.CreatedAt
            });

            var result = new PagedResult<SensorAdvertisementDto>
            {
                Items = sensorDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
            return ApiResponse<PagedResult<SensorAdvertisementDto>>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving medical device sensor data");
            return ApiResponse<PagedResult<SensorAdvertisementDto>>.ErrorResponse("Failed to retrieve medical device sensor data", 500);
        }
    }
}
