using System.IO;

namespace MedicalDeviceTracking.Application.Interfaces;
public interface ICloudImageService
{
    Task<(string url, string publicId, int? width, int? height, string? format)>
        UploadAsync(Stream content, string fileName, string contentType, string folder, CancellationToken ct = default);

    Task DeleteAsync(string publicId, CancellationToken ct = default);
}
