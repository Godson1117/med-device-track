using MedicalDeviceTracking.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDeviceTracking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResponse<T>(ApiResponse<T> response)
    {
        return response.StatusCode switch
        {
            200 => Ok(response),
            201 => Created("", response),
            400 => BadRequest(response),
            401 => Unauthorized(response),
            403 => Forbid(),
            404 => NotFound(response),
            500 => StatusCode(500, response),
            _ => StatusCode(response.StatusCode, response)
        };
    }
}
