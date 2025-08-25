using System.Text.Json.Serialization;

namespace MedicalDeviceTracking.Application.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonConstructor]
    public ApiResponse()
    {
    }

    public ApiResponse(bool success, string message, T? data = default, int statusCode = 200)
    {
        Success = success;
        Message = message;
        Data = data;
        StatusCode = statusCode;
    }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>(true, message, data, 200);
    }

    public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors ?? new List<string>()
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public ApiResponse() : base() { }

    public ApiResponse(bool success, string message, object? data = null, int statusCode = 200)
        : base(success, message, data, statusCode) { }

    public static ApiResponse SuccessResponse(string message = "Success")
    {
        return new ApiResponse(true, message, null, 200);
    }

    public static new ApiResponse ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors ?? new List<string>()
        };
    }
}
