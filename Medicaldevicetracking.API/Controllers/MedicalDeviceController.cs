using MedicalDeviceTracking.Application.DTOs;
using MedicalDeviceTracking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDeviceTracking.API.Controllers;

public class MedicalDeviceController : BaseController
{
    private readonly IKafkaDataService _kafkaDataService;
    private readonly ILogger<MedicalDeviceController> _logger;

    public MedicalDeviceController(IKafkaDataService kafkaDataService, ILogger<MedicalDeviceController> logger)
    {
        _kafkaDataService = kafkaDataService;
        _logger = logger;
    }

    /// <summary>
    /// Get all medical device gateways with pagination
    /// </summary>
    [HttpGet("gateways")]
    public async Task<IActionResult> GetGateways([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _kafkaDataService.GetGatewaysAsync(pageNumber, pageSize);
        return HandleResponse(response);
    }

    /// <summary>
    /// Get medical device gateway by ID
    /// </summary>
    [HttpGet("gateways/{id:guid}")]
    public async Task<IActionResult> GetGateway(Guid id)
    {
        var response = await _kafkaDataService.GetGatewayByIdAsync(id);
        return HandleResponse(response);
    }

    /// <summary>
    /// Get all medical device sensor data with pagination
    /// </summary>
    [HttpGet("sensors")]
    public async Task<IActionResult> GetSensorData([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _kafkaDataService.GetSensorDataAsync(pageNumber, pageSize);
        return HandleResponse(response);
    }

    /// <summary>
    /// Manually process a medical device Kafka message (for testing)
    /// </summary>
    [HttpPost("process-message")]
    public async Task<IActionResult> ProcessMessage([FromBody] KafkaMessageDto message)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(ApiResponse.ErrorResponse("Invalid medical device data input", 400, errors));
        }

        var response = await _kafkaDataService.ProcessKafkaMessageAsync(message);
        return HandleResponse(response);
    }
}
