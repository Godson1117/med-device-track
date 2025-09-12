// MedicalDeviceTracking.Infrastructure/Repositories/TagRepository.cs
using MedicalDeviceTracking.Domain.Entities;
using MedicalDeviceTracking.Domain.Interfaces;
using MedicalDeviceTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalDeviceTracking.Infrastructure.Repositories;
public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;
    public TagRepository(ApplicationDbContext context) => _context = context;

    public Task<Tag?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Tags.Include(t => t.CurrentGateway).FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<Tag?> GetByMacAsync(string mac, CancellationToken ct = default) =>
        _context.Tags.Include(t => t.CurrentGateway).FirstOrDefaultAsync(t => t.MacAddress == mac, ct);

    public Task<Tag?> GetByUuidAsync(string uuid, CancellationToken ct = default) =>
        _context.Tags.Include(t => t.CurrentGateway).FirstOrDefaultAsync(t => t.Uuid == uuid, ct);

    public async Task<IEnumerable<Tag>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default) =>
        await _context.Tags.AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<IEnumerable<Tag>> GetByGatewayAsync(Guid gatewayId, bool activeOnly, DateTime? from, DateTime? to, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var q = _context.Tags.AsNoTracking().Where(t => t.CurrentGatewayId == gatewayId);
        if (activeOnly) q = q.Where(t => t.Status == Domain.Enums.TagStatus.Active);
        if (from.HasValue) q = q.Where(t => t.LastSeenAt >= from);
        if (to.HasValue) q = q.Where(t => t.LastSeenAt <= to);
        return await q.OrderByDescending(t => t.LastSeenAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public Task<int> GetTotalCountAsync(CancellationToken ct = default) => _context.Tags.CountAsync(ct);
    public Task<int> GetActiveCountAsync(CancellationToken ct = default) => _context.Tags.CountAsync(t => t.Status == Domain.Enums.TagStatus.Active, ct);

    public async Task<Tag> AddAsync(Tag tag, CancellationToken ct = default)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(ct);
        return tag;
    }

    public async Task<Tag> UpdateAsync(Tag tag, CancellationToken ct = default)
    {
        tag.UpdatedAt = DateTime.UtcNow;
        _context.Tags.Update(tag);
        await _context.SaveChangesAsync(ct);
        return tag;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var found = await _context.Tags.FindAsync([id], ct);
        if (found != null)
        {
            _context.Tags.Remove(found);
            await _context.SaveChangesAsync(ct);
        }
    }
}
