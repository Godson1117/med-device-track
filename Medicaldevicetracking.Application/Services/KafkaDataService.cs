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
    private readonly IMapper _mapper;
    private readonly ILogger<KafkaDataService> _logger;

    public KafkaDataService(
        IGatewayRepository gatewayRepository,
        ISensorAdvertisementRepository sensorRepository,
        IMapper mapper,
        ILogger<KafkaDataService> logger)
    {
        _gatewayRepository = gatewayRepository;
        _sensorRepository = sensorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<GatewayDto>> ProcessKafkaMessageAsync(KafkaMessageDto message)
    {
        try
        {
            _logger.LogInformation("Processing Kafka message for medical device gateway: {GatewayId}", message.Gateway);

            // Parse timestamp and ensure UTC
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

            // Check if gateway exists
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

            // Process medical device advertisements
            var advertisements = new List<SensorAdvertisement>();

            foreach (var adv in message.Advertisements)
            {
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

                var sensorAdv = new SensorAdvertisement
                {
                    Id = Guid.NewGuid(),
                    GatewayId = gateway.Id,
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

            // **FIX: Properly declare and assign the 'result' variable**
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

            // Manual mapping for now
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

            // Manual mapping for now
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

            // Manual mapping for now
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
