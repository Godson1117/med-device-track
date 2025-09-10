// MedicalDeviceTracking.Domain/Interfaces/IFloorMapRepository.cs
using MedicalDeviceTracking.Domain.Entities;

namespace MedicalDeviceTracking.Domain.Interfaces;
public interface IFloorMapRepository
{
    Task<FloorMap?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<FloorMap>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<FloorMap> AddAsync(FloorMap map, CancellationToken ct = default);
    Task<FloorMap> UpdateAsync(FloorMap map, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
