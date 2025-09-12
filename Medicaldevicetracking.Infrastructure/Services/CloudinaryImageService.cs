using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MedicalDeviceTracking.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql.BackendMessages;

namespace MedicalDeviceTracking.Infrastructure.Services;
public class CloudinaryImageService : ICloudImageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageService(IConfiguration config)
    {
        // CLOUDINARY_URL=cloudinary://<api_key>:<api_secret>@<cloud_name>
        var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL")
                            ?? config["CLOUDINARY_URL"];
        if (string.IsNullOrWhiteSpace(cloudinaryUrl))
            throw new InvalidOperationException("CLOUDINARY_URL is not configured.");

        _cloudinary = new Cloudinary(cloudinaryUrl);
        _cloudinary.Api.Secure = true;
    }

    public async Task<(string url, string publicId, int? width, int? height, string? format)>
        UploadAsync(Stream content, string fileName, string contentType, string folder, CancellationToken ct = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, content),
            Folder = folder,
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false,
            Transformation = new Transformation().FetchFormat("auto").Quality("auto")
        };

        // Some package versions do not accept a CancellationToken
        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.StatusCode is not System.Net.HttpStatusCode.OK)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error?.Message}");

        return (result.SecureUrl?.ToString() ?? string.Empty,
                result.PublicId ?? string.Empty,
                result.Width, result.Height,
                result.Format);
    }

    public async Task DeleteAsync(string publicId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(publicId)) return;

        var delParams = new DeletionParams(publicId)
        {
            Invalidate = true,
            ResourceType = ResourceType.Image
        };
        // Some package versions do not accept a CancellationToken
        await _cloudinary.DestroyAsync(delParams);
    }
}
