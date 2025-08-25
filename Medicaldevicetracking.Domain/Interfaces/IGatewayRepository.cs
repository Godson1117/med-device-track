using MedicalDeviceTracking.Domain.Entities;

namespace MedicalDeviceTracking.Domain.Interfaces;

public interface IGatewayRepository
{
    Task<Gateway?> GetByIdAsync(Guid id);
    Task<Gateway?> GetByGatewayIdAsync(string gatewayId);
    Task<IEnumerable<Gateway>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<int> GetTotalCountAsync(); // Add this method
    Task<Gateway> AddAsync(Gateway gateway);
    Task<Gateway> UpdateAsync(Gateway gateway);
    Task DeleteAsync(Guid id);
}
