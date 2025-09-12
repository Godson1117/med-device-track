using MedicalDeviceTracking.Application.DTOs;
using MedicalDeviceTracking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDeviceTracking.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FloorMapsController : BaseController
{
    private readonly IFloorMapService _service;
    private readonly ICloudImageService _images;

    public FloorMapsController(IFloorMapService service, ICloudImageService images)
    {
        _service = service;
        _images = images;
    }

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

    // ---------- New: Create with file (multipart/form-data) ----------
    public class FloorMapCreateForm
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public bool IsActive { get; set; } = true;
        public IFormFile Image { get; set; } = default!;
    }

    [HttpPost("create-with-file")]
    [RequestSizeLimit(30_000_000)] // 30 MB
    public async Task<IActionResult> CreateWithFile([FromForm] FloorMapCreateForm form, CancellationToken ct)
    {
        if (!ModelState.IsValid || form.Image == null || form.Image.Length == 0)
            return BadRequest(ApiResponse.ErrorResponse("Invalid floor map input", 400));

        if (!form.Image.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return BadRequest(ApiResponse.ErrorResponse("Only image uploads are allowed", 400));

        await using var stream = form.Image.OpenReadStream();
        var (url, publicId, _, _, _) = await _images.UploadAsync(
            stream,
            form.Image.FileName,
            form.Image.ContentType ?? "application/octet-stream",
            folder: "floormaps",
            ct: ct);

        var dto = new FloorMapCreateDto
        {
            Name = form.Name,
            Description = form.Description,
            ImagePath = url,
            Width = form.Width,
            Height = form.Height,
            IsActive = form.IsActive
        };

        var response = await _service.CreateAsync(dto);
        return response.Success ? Created("", response) : HandleResponse(response);
    }
}
