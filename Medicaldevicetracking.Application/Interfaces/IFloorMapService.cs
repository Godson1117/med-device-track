// Application/Interfaces/IFloorMapService.cs
using MedicalDeviceTracking.Application.DTOs;

namespace MedicalDeviceTracking.Application.Interfaces;
public interface IFloorMapService
{
    Task<ApiResponse<PagedResult<FloorMapReadDto>>> GetAllAsync(int pageNumber, int pageSize);
    Task<ApiResponse<FloorMapReadDto?>> GetByIdAsync(Guid id);
    Task<ApiResponse<FloorMapReadDto>> CreateAsync(FloorMapCreateDto dto);
    Task<ApiResponse<FloorMapReadDto>> UpdateAsync(Guid id, FloorMapUpdateDto dto);
    Task<ApiResponse> DeleteAsync(Guid id);
}
