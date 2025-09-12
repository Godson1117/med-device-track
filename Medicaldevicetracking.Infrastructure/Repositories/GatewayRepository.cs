// MedicalDeviceTracking.Infrastructure/Repositories/GatewayRepository.cs (updated)
using MedicalDeviceTracking.Domain.Entities;
using MedicalDeviceTracking.Domain.Interfaces;
using MedicalDeviceTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalDeviceTracking.Infrastructure.Repositories;
public class GatewayRepository : IGatewayRepository
{
    private readonly ApplicationDbContext _context;
    public GatewayRepository(ApplicationDbContext context) => _context = context;

    public async Task<Gateway?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Gateways.Include(g => g.FloorMap).FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<Gateway?> GetByGatewayIdAsync(string gatewayId, CancellationToken ct = default) =>
        await _context.Gateways.FirstOrDefaultAsync(g => g.GatewayId == gatewayId, ct);

    public async Task<Gateway?> GetByMacAsync(string mac, CancellationToken ct = default) =>
        await _context.Gateways.FirstOrDefaultAsync(g => g.MacAddress == mac, ct);

    public async Task<Gateway?> GetByUuidAsync(string uuid, CancellationToken ct = default) =>
        await _context.Gateways.FirstOrDefaultAsync(g => g.Uuid == uuid, ct);

    public async Task<IEnumerable<Gateway>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default) =>
        await _context.Gateways.AsNoTracking()
            .OrderByDescending(g => g.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public Task<int> GetTotalCountAsync(CancellationToken ct = default) =>
        _context.Gateways.CountAsync(ct);

    public Task<int> GetActiveCountAsync(CancellationToken ct = default) =>
        _context.Gateways.CountAsync(g => g.Status == Domain.Enums.GatewayStatus.Active, ct);

    public async Task<Gateway> AddAsync(Gateway gateway, CancellationToken ct = default)
    {
        _context.Gateways.Add(gateway);
        await _context.SaveChangesAsync(ct);
        return gateway;
    }

    public async Task<Gateway> UpdateAsync(Gateway gateway, CancellationToken ct = default)
    {
        gateway.UpdatedAt = DateTime.UtcNow;
        _context.Gateways.Update(gateway);
        await _context.SaveChangesAsync(ct);
        return gateway;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var found = await _context.Gateways.FindAsync([id], ct);
        if (found != null)
        {
            _context.Gateways.Remove(found);
            await _context.SaveChangesAsync(ct);
        }
    }
}
