// MedicalDeviceTracking.Domain/Interfaces/IGatewayRepository.cs (extended)
using MedicalDeviceTracking.Domain.Entities;

namespace MedicalDeviceTracking.Domain.Interfaces;
public interface IGatewayRepository
{
    Task<Gateway?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Gateway?> GetByGatewayIdAsync(string gatewayId, CancellationToken ct = default);
    Task<Gateway?> GetByMacAsync(string mac, CancellationToken ct = default);
    Task<Gateway?> GetByUuidAsync(string uuid, CancellationToken ct = default);
    Task<IEnumerable<Gateway>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<int> GetActiveCountAsync(CancellationToken ct = default);
    Task<Gateway> AddAsync(Gateway gateway, CancellationToken ct = default);
    Task<Gateway> UpdateAsync(Gateway gateway, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
