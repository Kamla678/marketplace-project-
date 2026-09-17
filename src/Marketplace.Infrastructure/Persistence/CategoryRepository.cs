using Microsoft.EntityFrameworkCore;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces;

namespace Marketplace.Infrastructure.Persistence;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Categories.ToListAsync(ct);

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default) =>
        await _context.Categories.AnyAsync(c => c.Slug == slug, ct);

    public async Task AddAsync(Category category, CancellationToken ct = default)
    {
        await _context.Categories.AddAsync(category, ct);
        await _context.SaveChangesAsync(ct);
    }
}
