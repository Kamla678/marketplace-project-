namespace Marketplace.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public string ImageUrl { get; set; } = default!;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}
