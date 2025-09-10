// MedicalDeviceTracking.Infrastructure/Repositories/FloorMapRepository.cs
using MedicalDeviceTracking.Domain.Entities;
using MedicalDeviceTracking.Domain.Interfaces;
using MedicalDeviceTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalDeviceTracking.Infrastructure.Repositories;
public class FloorMapRepository : IFloorMapRepository
{
    private readonly ApplicationDbContext _context;
    public FloorMapRepository(ApplicationDbContext context) => _context = context;

    public Task<FloorMap?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.FloorMaps.FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<IEnumerable<FloorMap>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default) =>
        await _context.FloorMaps.AsNoTracking()
            .OrderByDescending(f => f.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public Task<int> GetTotalCountAsync(CancellationToken ct = default) =>
        _context.FloorMaps.CountAsync(ct);

    public async Task<FloorMap> AddAsync(FloorMap map, CancellationToken ct = default)
    {
        _context.FloorMaps.Add(map);
        await _context.SaveChangesAsync(ct);
        return map;
    }

    public async Task<FloorMap> UpdateAsync(FloorMap map, CancellationToken ct = default)
    {
        map.UpdatedAt = DateTime.UtcNow;
        _context.FloorMaps.Update(map);
        await _context.SaveChangesAsync(ct);
        return map;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var found = await _context.FloorMaps.FindAsync([id], ct);
        if (found != null)
        {
            _context.FloorMaps.Remove(found);
            await _context.SaveChangesAsync(ct);
        }
    }
}
