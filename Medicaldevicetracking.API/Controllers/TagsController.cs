// API/Controllers/TagsController.cs
using MedicalDeviceTracking.Application.DTOs;
using MedicalDeviceTracking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDeviceTracking.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TagsController : BaseController
{
    private readonly ITagService _service;
    public TagsController(ITagService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) =>
        HandleResponse(await _service.GetAllAsync(pageNumber, pageSize));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) =>
        HandleResponse(await _service.GetByIdAsync(id));

    [HttpGet("by-mac/{mac}")]
    public async Task<IActionResult> GetByMac(string mac) =>
        HandleResponse(await _service.GetByMacAsync(mac));

    [HttpGet("by-uuid/{uuid}")]
    public async Task<IActionResult> GetByUuid(string uuid) =>
        HandleResponse(await _service.GetByUuidAsync(uuid));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Invalid tag input", 400));
        var response = await _service.CreateAsync(dto);
        return response.Success ? Created("", response) : HandleResponse(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TagUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Invalid tag input", 400));
        return HandleResponse(await _service.UpdateAsync(id, dto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) =>
        HandleResponse(await _service.DeleteAsync(id));

    [HttpGet("location/by-mac/{mac}")]
    public async Task<IActionResult> GetLocationByMac(string mac) =>
        HandleResponse(await _service.GetLocationByTagMacAsync(mac));

    [HttpGet("stats")]
    public async Task<IActionResult> Stats() =>
        HandleResponse(await _service.GetStatsAsync());
}
