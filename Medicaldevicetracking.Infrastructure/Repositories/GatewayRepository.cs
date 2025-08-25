using MedicalDeviceTracking.Domain.Entities;
using MedicalDeviceTracking.Domain.Interfaces;
using MedicalDeviceTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalDeviceTracking.Infrastructure.Repositories;

public class GatewayRepository : IGatewayRepository
{
    private readonly ApplicationDbContext _context;

    public GatewayRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Gateway?> GetByIdAsync(Guid id)
    {
        return await _context.Gateways
            .Include(g => g.Advertisements)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Gateway?> GetByGatewayIdAsync(string gatewayId)
    {
        return await _context.Gateways
            .Include(g => g.Advertisements)
            .FirstOrDefaultAsync(g => g.GatewayId == gatewayId);
    }

    public async Task<IEnumerable<Gateway>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await _context.Gateways
            .Include(g => g.Advertisements)
            .OrderByDescending(g => g.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Gateways.CountAsync();
    }

    public async Task<Gateway> AddAsync(Gateway gateway)
    {
        _context.Gateways.Add(gateway);
        await _context.SaveChangesAsync();
        return gateway;
    }

    public async Task<Gateway> UpdateAsync(Gateway gateway)
    {
        gateway.UpdatedAt = DateTime.UtcNow;
        _context.Gateways.Update(gateway);
        await _context.SaveChangesAsync();
        return gateway;
    }

    public async Task DeleteAsync(Guid id)
    {
        var gateway = await _context.Gateways.FindAsync(id);
        if (gateway != null)
        {
            _context.Gateways.Remove(gateway);
            await _context.SaveChangesAsync();
        }
    }
}
