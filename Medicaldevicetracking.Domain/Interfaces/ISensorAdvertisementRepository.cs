using MedicalDeviceTracking.Domain.Entities;

namespace MedicalDeviceTracking.Domain.Interfaces;

public interface ISensorAdvertisementRepository
{
    Task<SensorAdvertisement?> GetByIdAsync(Guid id);
    Task<IEnumerable<SensorAdvertisement>> GetByGatewayIdAsync(Guid gatewayId);
    Task<IEnumerable<SensorAdvertisement>> GetByMacAddressAsync(string macAddress);
    Task<IEnumerable<SensorAdvertisement>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<int> GetTotalCountAsync(); // Add this method
    Task<SensorAdvertisement> AddAsync(SensorAdvertisement advertisement);
    Task<IEnumerable<SensorAdvertisement>> AddRangeAsync(IEnumerable<SensorAdvertisement> advertisements);
    Task<SensorAdvertisement> UpdateAsync(SensorAdvertisement advertisement);
    Task DeleteAsync(Guid id);
}
