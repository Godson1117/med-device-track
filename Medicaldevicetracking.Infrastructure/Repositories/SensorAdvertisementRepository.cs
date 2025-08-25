using MedicalDeviceTracking.Domain.Entities;
using MedicalDeviceTracking.Domain.Interfaces;
using MedicalDeviceTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalDeviceTracking.Infrastructure.Repositories;

public class SensorAdvertisementRepository : ISensorAdvertisementRepository
{
    private readonly ApplicationDbContext _context;

    public SensorAdvertisementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SensorAdvertisement?> GetByIdAsync(Guid id)
    {
        return await _context.SensorAdvertisements
            .Include(s => s.Gateway)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<SensorAdvertisement>> GetByGatewayIdAsync(Guid gatewayId)
    {
        return await _context.SensorAdvertisements
            .Where(s => s.GatewayId == gatewayId)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<SensorAdvertisement>> GetByMacAddressAsync(string macAddress)
    {
        return await _context.SensorAdvertisements
            .Include(s => s.Gateway)
            .Where(s => s.MacAddress == macAddress)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<SensorAdvertisement>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await _context.SensorAdvertisements
            .Include(s => s.Gateway)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.SensorAdvertisements.CountAsync();
    }

    public async Task<SensorAdvertisement> AddAsync(SensorAdvertisement advertisement)
    {
        _context.SensorAdvertisements.Add(advertisement);
        await _context.SaveChangesAsync();
        return advertisement;
    }

    public async Task<IEnumerable<SensorAdvertisement>> AddRangeAsync(IEnumerable<SensorAdvertisement> advertisements)
    {
        _context.SensorAdvertisements.AddRange(advertisements);
        await _context.SaveChangesAsync();
        return advertisements;
    }

    public async Task<SensorAdvertisement> UpdateAsync(SensorAdvertisement advertisement)
    {
        advertisement.UpdatedAt = DateTime.UtcNow;
        _context.SensorAdvertisements.Update(advertisement);
        await _context.SaveChangesAsync();
        return advertisement;
    }

    public async Task DeleteAsync(Guid id)
    {
        var advertisement = await _context.SensorAdvertisements.FindAsync(id);
        if (advertisement != null)
        {
            _context.SensorAdvertisements.Remove(advertisement);
            await _context.SaveChangesAsync();
        }
    }
}
