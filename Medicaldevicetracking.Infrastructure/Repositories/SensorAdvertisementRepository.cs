// MedicalDeviceTracking.Infrastructure/Repositories/SensorAdvertisementRepository.cs (updated)
using MedicalDeviceTracking.Domain.Entities;
using MedicalDeviceTracking.Domain.Interfaces;
using MedicalDeviceTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalDeviceTracking.Infrastructure.Repositories;
public class SensorAdvertisementRepository : ISensorAdvertisementRepository
{
    private readonly ApplicationDbContext _context;
    public SensorAdvertisementRepository(ApplicationDbContext context) => _context = context;

    public Task<SensorAdvertisement?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.SensorAdvertisements.Include(s => s.Gateway).Include(s => s.Tag).FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IEnumerable<SensorAdvertisement>> GetByGatewayIdAsync(Guid gatewayId, DateTime? from, DateTime? to, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var q = _context.SensorAdvertisements.AsNoTracking().Where(s => s.GatewayId == gatewayId);
        if (from.HasValue) q = q.Where(s => s.Timestamp >= from);
        if (to.HasValue) q = q.Where(s => s.Timestamp <= to);
        return await q.OrderByDescending(s => s.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<SensorAdvertisement>> GetByTagIdAsync(Guid tagId, DateTime? from, DateTime? to, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var q = _context.SensorAdvertisements.AsNoTracking().Where(s => s.TagId == tagId);
        if (from.HasValue) q = q.Where(s => s.Timestamp >= from);
        if (to.HasValue) q = q.Where(s => s.Timestamp <= to);
        return await q.OrderByDescending(s => s.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<SensorAdvertisement>> GetByMacAddressAsync(string macAddress, int pageNumber, int pageSize, CancellationToken ct = default) =>
        await _context.SensorAdvertisements.AsNoTracking()
            .Where(s => s.MacAddress == macAddress)
            .OrderByDescending(s => s.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<IEnumerable<SensorAdvertisement>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default) =>
        await _context.SensorAdvertisements.AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public Task<int> GetTotalCountAsync(CancellationToken ct = default) =>
        _context.SensorAdvertisements.CountAsync(ct);

    public async Task<SensorAdvertisement> AddAsync(SensorAdvertisement advertisement, CancellationToken ct = default)
    {
        _context.SensorAdvertisements.Add(advertisement);
        await _context.SaveChangesAsync(ct);
        return advertisement;
    }

    public async Task<IEnumerable<SensorAdvertisement>> AddRangeAsync(IEnumerable<SensorAdvertisement> advertisements, CancellationToken ct = default)
    {
        _context.SensorAdvertisements.AddRange(advertisements);
        await _context.SaveChangesAsync(ct);
        return advertisements;
    }

    public async Task<SensorAdvertisement> UpdateAsync(SensorAdvertisement advertisement, CancellationToken ct = default)
    {
        advertisement.UpdatedAt = DateTime.UtcNow;
        _context.SensorAdvertisements.Update(advertisement);
        await _context.SaveChangesAsync(ct);
        return advertisement;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var found = await _context.SensorAdvertisements.FindAsync([id], ct);
        if (found != null)
        {
            _context.SensorAdvertisements.Remove(found);
            await _context.SaveChangesAsync(ct);
        }
    }
}
