// MedicalDeviceTracking.Domain/Interfaces/ISensorAdvertisementRepository.cs (extended)
using MedicalDeviceTracking.Domain.Entities;

namespace MedicalDeviceTracking.Domain.Interfaces;
public interface ISensorAdvertisementRepository
{
    Task<SensorAdvertisement?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<SensorAdvertisement>> GetByGatewayIdAsync(Guid gatewayId, DateTime? from, DateTime? to, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<IEnumerable<SensorAdvertisement>> GetByTagIdAsync(Guid tagId, DateTime? from, DateTime? to, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<IEnumerable<SensorAdvertisement>> GetByMacAddressAsync(string macAddress, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<IEnumerable<SensorAdvertisement>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<SensorAdvertisement> AddAsync(SensorAdvertisement advertisement, CancellationToken ct = default);
    Task<IEnumerable<SensorAdvertisement>> AddRangeAsync(IEnumerable<SensorAdvertisement> advertisements, CancellationToken ct = default);
    Task<SensorAdvertisement> UpdateAsync(SensorAdvertisement advertisement, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
