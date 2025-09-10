// Application/Services/GatewayService.cs
using AutoMapper;
using MedicalDeviceTracking.Application.DTOs;
using MedicalDeviceTracking.Application.Interfaces;
using MedicalDeviceTracking.Domain.Entities;
using MedicalDeviceTracking.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicalDeviceTracking.Application.Services;
public class GatewayService : IGatewayService
{
    private readonly IGatewayRepository _repo;
    private readonly ITagRepository _tagRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<GatewayService> _logger;

    public GatewayService(IGatewayRepository repo, ITagRepository tagRepo, IMapper mapper, ILogger<GatewayService> logger)
    {
        _repo = repo; _tagRepo = tagRepo; _mapper = mapper; _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<GatewayReadDto>>> GetAllAsync(int pageNumber, int pageSize)
    {
        var items = await _repo.GetAllAsync(pageNumber, pageSize);
        var total = await _repo.GetTotalCountAsync();
        var dtos = items.Select(_mapper.Map<GatewayReadDto>);
        var result = new PagedResult<GatewayReadDto> { Items = dtos, PageNumber = pageNumber, PageSize = pageSize, TotalCount = total };
        return ApiResponse<PagedResult<GatewayReadDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<GatewayReadDto?>> GetByIdAsync(Guid id)
    {
        var g = await _repo.GetByIdAsync(id);
        return g == null
            ? ApiResponse<GatewayReadDto?>.ErrorResponse("Gateway not found", 404)
            : ApiResponse<GatewayReadDto?>.SuccessResponse(_mapper.Map<GatewayReadDto>(g));
    }

    public async Task<ApiResponse<GatewayReadDto?>> GetByMacAsync(string mac)
    {
        var g = await _repo.GetByMacAsync(mac);
        return g == null
            ? ApiResponse<GatewayReadDto?>.ErrorResponse("Gateway not found", 404)
            : ApiResponse<GatewayReadDto?>.SuccessResponse(_mapper.Map<GatewayReadDto>(g));
    }

    public async Task<ApiResponse<GatewayReadDto?>> GetByUuidAsync(string uuid)
    {
        var g = await _repo.GetByUuidAsync(uuid);
        return g == null
            ? ApiResponse<GatewayReadDto?>.ErrorResponse("Gateway not found", 404)
            : ApiResponse<GatewayReadDto?>.SuccessResponse(_mapper.Map<GatewayReadDto>(g));
    }

    public async Task<ApiResponse<GatewayReadDto>> CreateAsync(GatewayCreateDto dto)
    {
        var entity = _mapper.Map<Gateway>(dto);
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        var saved = await _repo.AddAsync(entity);
        return ApiResponse<GatewayReadDto>.SuccessResponse(_mapper.Map<GatewayReadDto>(saved), "Created");
    }

    public async Task<ApiResponse<GatewayReadDto>> UpdateAsync(Guid id, GatewayUpdateDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return ApiResponse<GatewayReadDto>.ErrorResponse("Gateway not found", 404);
        _mapper.Map(dto, existing);
        var saved = await _repo.UpdateAsync(existing);
        return ApiResponse<GatewayReadDto>.SuccessResponse(_mapper.Map<GatewayReadDto>(saved), "Updated");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        await _repo.DeleteAsync(id);
        return ApiResponse.SuccessResponse("Deleted");
    }

    public async Task<ApiResponse<object>> GetStatsAsync()
    {
        var total = await _repo.GetTotalCountAsync();
        var active = await _repo.GetActiveCountAsync();
        return ApiResponse<object>.SuccessResponse(new { totalGateways = total, activeGateways = active });
    }

    public async Task<ApiResponse<PagedResult<TagReadDto>>> GetTagsForGatewayAsync(Guid gatewayId, bool activeOnly, DateTime? from, DateTime? to, int pageNumber, int pageSize)
    {
        var tags = await _tagRepo.GetByGatewayAsync(gatewayId, activeOnly, from, to, pageNumber, pageSize);
        var total = tags.Count(); // For simple paging; for accurate counts, add a count query
        var dtos = tags.Select(_mapper.Map<TagReadDto>);
        var result = new PagedResult<TagReadDto> { Items = dtos, PageNumber = pageNumber, PageSize = pageSize, TotalCount = total };
        return ApiResponse<PagedResult<TagReadDto>>.SuccessResponse(result);
    }
}
