// Application/Interfaces/IGatewayService.cs
using MedicalDeviceTracking.Application.DTOs;

namespace MedicalDeviceTracking.Application.Interfaces;
public interface IGatewayService
{
    Task<ApiResponse<PagedResult<GatewayReadDto>>> GetAllAsync(int pageNumber, int pageSize);
    Task<ApiResponse<GatewayReadDto?>> GetByIdAsync(Guid id);
    Task<ApiResponse<GatewayReadDto?>> GetByMacAsync(string mac);
    Task<ApiResponse<GatewayReadDto?>> GetByUuidAsync(string uuid);
    Task<ApiResponse<GatewayReadDto>> CreateAsync(GatewayCreateDto dto);
    Task<ApiResponse<GatewayReadDto>> UpdateAsync(Guid id, GatewayUpdateDto dto);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse<object>> GetStatsAsync();
    Task<ApiResponse<PagedResult<TagReadDto>>> GetTagsForGatewayAsync(Guid gatewayId, bool activeOnly, DateTime? from, DateTime? to, int pageNumber, int pageSize);
}
