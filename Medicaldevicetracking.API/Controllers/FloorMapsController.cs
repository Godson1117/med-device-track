// API/Controllers/FloorMapsController.cs
using MedicalDeviceTracking.Application.DTOs;
using MedicalDeviceTracking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDeviceTracking.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FloorMapsController : BaseController
{
    private readonly IFloorMapService _service;
    public FloorMapsController(IFloorMapService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) =>
        HandleResponse(await _service.GetAllAsync(pageNumber, pageSize));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) =>
        HandleResponse(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FloorMapCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Invalid floor map input", 400));
        var response = await _service.CreateAsync(dto);
        return response.Success ? Created("", response) : HandleResponse(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] FloorMapUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Invalid floor map input", 400));
        return HandleResponse(await _service.UpdateAsync(id, dto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) =>
        HandleResponse(await _service.DeleteAsync(id));
}
