using MedicalDeviceTracking.Application.DTOs;

namespace MedicalDeviceTracking.Application.Interfaces;

public interface IKafkaDataService
{
    Task<ApiResponse<GatewayDto>> ProcessKafkaMessageAsync(KafkaMessageDto message);
    Task<ApiResponse<PagedResult<GatewayDto>>> GetGatewaysAsync(int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse<GatewayDto?>> GetGatewayByIdAsync(Guid id);
    Task<ApiResponse<PagedResult<SensorAdvertisementDto>>> GetSensorDataAsync(int pageNumber = 1, int pageSize = 10);
}
