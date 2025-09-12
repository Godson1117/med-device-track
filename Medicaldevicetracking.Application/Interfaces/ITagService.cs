// Application/Interfaces/ITagService.cs
using MedicalDeviceTracking.Application.DTOs;

namespace MedicalDeviceTracking.Application.Interfaces;
public interface ITagService
{
    Task<ApiResponse<PagedResult<TagReadDto>>> GetAllAsync(int pageNumber, int pageSize);
    Task<ApiResponse<TagReadDto?>> GetByIdAsync(Guid id);
    Task<ApiResponse<TagReadDto?>> GetByMacAsync(string mac);
    Task<ApiResponse<TagReadDto?>> GetByUuidAsync(string uuid);
    Task<ApiResponse<TagReadDto>> CreateAsync(TagCreateDto dto);
    Task<ApiResponse<TagReadDto>> UpdateAsync(Guid id, TagUpdateDto dto);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse<object>> GetStatsAsync();
    Task<ApiResponse<object>> GetLocationByTagMacAsync(string mac);
}
