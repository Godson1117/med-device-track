// Application/Services/TagService.cs
using AutoMapper;
using MedicalDeviceTracking.Application.DTOs;
using MedicalDeviceTracking.Application.Interfaces;
using MedicalDeviceTracking.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicalDeviceTracking.Application.Services;
public class TagService : ITagService
{
    private readonly ITagRepository _repo;
    private readonly IGatewayRepository _gatewayRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<TagService> _logger;

    public TagService(ITagRepository repo, IGatewayRepository gatewayRepo, IMapper mapper, ILogger<TagService> logger)
    {
        _repo = repo; _gatewayRepo = gatewayRepo; _mapper = mapper; _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<TagReadDto>>> GetAllAsync(int pageNumber, int pageSize)
    {
        var items = await _repo.GetAllAsync(pageNumber, pageSize);
        var total = await _repo.GetTotalCountAsync();
        var dtos = items.Select(_mapper.Map<TagReadDto>);
        var result = new PagedResult<TagReadDto> { Items = dtos, PageNumber = pageNumber, PageSize = pageSize, TotalCount = total };
        return ApiResponse<PagedResult<TagReadDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<TagReadDto?>> GetByIdAsync(Guid id)
    {
        var t = await _repo.GetByIdAsync(id);
        return t == null
            ? ApiResponse<TagReadDto?>.ErrorResponse("Tag not found", 404)
            : ApiResponse<TagReadDto?>.SuccessResponse(_mapper.Map<TagReadDto>(t));
    }

    public async Task<ApiResponse<TagReadDto?>> GetByMacAsync(string mac)
    {
        var t = await _repo.GetByMacAsync(mac);
        return t == null
            ? ApiResponse<TagReadDto?>.ErrorResponse("Tag not found", 404)
            : ApiResponse<TagReadDto?>.SuccessResponse(_mapper.Map<TagReadDto>(t));
    }

    public async Task<ApiResponse<TagReadDto?>> GetByUuidAsync(string uuid)
    {
        var t = await _repo.GetByUuidAsync(uuid);
        return t == null
            ? ApiResponse<TagReadDto?>.ErrorResponse("Tag not found", 404)
            : ApiResponse<TagReadDto?>.SuccessResponse(_mapper.Map<TagReadDto>(t));
    }

    public async Task<ApiResponse<TagReadDto>> CreateAsync(TagCreateDto dto)
    {
        var entity = _mapper.Map<Domain.Entities.Tag>(dto);
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        var saved = await _repo.AddAsync(entity);
        return ApiResponse<TagReadDto>.SuccessResponse(_mapper.Map<TagReadDto>(saved), "Created");
    }

    public async Task<ApiResponse<TagReadDto>> UpdateAsync(Guid id, TagUpdateDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return ApiResponse<TagReadDto>.ErrorResponse("Tag not found", 404);
        _mapper.Map(dto, existing);
        var saved = await _repo.UpdateAsync(existing);
        return ApiResponse<TagReadDto>.SuccessResponse(_mapper.Map<TagReadDto>(saved), "Updated");
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
        return ApiResponse<object>.SuccessResponse(new { totalTags = total, activeTags = active });
    }

    public async Task<ApiResponse<object>> GetLocationByTagMacAsync(string mac)
    {
        var tag = await _repo.GetByMacAsync(mac);
        if (tag == null) return ApiResponse<object>.ErrorResponse("Tag not found", 404);
        var gw = tag.CurrentGatewayId.HasValue ? await _gatewayRepo.GetByIdAsync(tag.CurrentGatewayId.Value) : null;
        var payload = new
        {
            tagId = tag.Id,
            tagMac = tag.MacAddress,
            currentGatewayId = gw?.Id,
            gatewayMac = gw?.MacAddress,
            gatewayName = gw?.GatewayName,
            floorMapId = gw?.FloorMapId,
            lastRssi = tag.LastRssi,
            lastSeenAt = tag.LastSeenAt
        };
        return ApiResponse<object>.SuccessResponse(payload);
    }
}
