using Marketplace.Domain.Entities;

namespace Marketplace.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(CancellationToken ct = default); // الشجرة كاملة، بتتبني هرميًا في الـApplication layer
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
    Task AddAsync(Category category, CancellationToken ct = default);
}
