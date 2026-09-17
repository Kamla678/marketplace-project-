namespace Marketplace.Domain.Entities;

public class Brand : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? LogoUrl { get; set; }
}
