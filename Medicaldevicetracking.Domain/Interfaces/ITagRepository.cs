// MedicalDeviceTracking.Domain/Interfaces/ITagRepository.cs
using MedicalDeviceTracking.Domain.Entities;

namespace MedicalDeviceTracking.Domain.Interfaces;
public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Tag?> GetByMacAsync(string mac, CancellationToken ct = default);
    Task<Tag?> GetByUuidAsync(string uuid, CancellationToken ct = default);
    Task<IEnumerable<Tag>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
    Task<IEnumerable<Tag>> GetByGatewayAsync(Guid gatewayId, bool activeOnly, DateTime? from, DateTime? to, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<int> GetActiveCountAsync(CancellationToken ct = default);
    Task<Tag> AddAsync(Tag tag, CancellationToken ct = default);
    Task<Tag> UpdateAsync(Tag tag, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
