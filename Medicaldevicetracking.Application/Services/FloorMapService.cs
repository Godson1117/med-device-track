// Application/Services/FloorMapService.cs
using AutoMapper;
using MedicalDeviceTracking.Application.DTOs;
using MedicalDeviceTracking.Application.Interfaces;
using MedicalDeviceTracking.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicalDeviceTracking.Application.Services;
public class FloorMapService : IFloorMapService
{
    private readonly IFloorMapRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<FloorMapService> _logger;
    public FloorMapService(IFloorMapRepository repo, IMapper mapper, ILogger<FloorMapService> logger)
    { _repo = repo; _mapper = mapper; _logger = logger; }

    public async Task<ApiResponse<PagedResult<FloorMapReadDto>>> GetAllAsync(int pageNumber, int pageSize)
    {
        var items = await _repo.GetAllAsync(pageNumber, pageSize);
        var total = await _repo.GetTotalCountAsync();
        var dtos = items.Select(_mapper.Map<FloorMapReadDto>);
        var result = new PagedResult<FloorMapReadDto> { Items = dtos, PageNumber = pageNumber, PageSize = pageSize, TotalCount = total };
        return ApiResponse<PagedResult<FloorMapReadDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<FloorMapReadDto?>> GetByIdAsync(Guid id)
    {
        var f = await _repo.GetByIdAsync(id);
        return f == null
            ? ApiResponse<FloorMapReadDto?>.ErrorResponse("Floor map not found", 404)
            : ApiResponse<FloorMapReadDto?>.SuccessResponse(_mapper.Map<FloorMapReadDto>(f));
    }

    public async Task<ApiResponse<FloorMapReadDto>> CreateAsync(FloorMapCreateDto dto)
    {
        var entity = _mapper.Map<Domain.Entities.FloorMap>(dto);
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        var saved = await _repo.AddAsync(entity);
        return ApiResponse<FloorMapReadDto>.SuccessResponse(_mapper.Map<FloorMapReadDto>(saved), "Created");
    }

    public async Task<ApiResponse<FloorMapReadDto>> UpdateAsync(Guid id, FloorMapUpdateDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return ApiResponse<FloorMapReadDto>.ErrorResponse("Floor map not found", 404);
        _mapper.Map(dto, existing);
        var saved = await _repo.UpdateAsync(existing);
        return ApiResponse<FloorMapReadDto>.SuccessResponse(_mapper.Map<FloorMapReadDto>(saved), "Updated");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        await _repo.DeleteAsync(id);
        return ApiResponse.SuccessResponse("Deleted");
    }
}
