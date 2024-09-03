

using Ecommerce.Shared.Entities.ProductVariants;
namespace Ecommerce.Shared.Dto;

public class ProductVariantDetailDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public long TemplateMasterId { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string? Color { get; set; }
    public string ?Size { get; set; }
    public int year { get; set; }
    
    public double ProductPrice { get; set; }
    public double VariantPrice { get; set; }
    public string? Sku { get; set; }
    public string? Value { get; set; }
    public string? SSN { get; set; }
    public string? Description { get; set; }
    public int discountPercentage { get; set; }
    public bool Publish { get; set; }
    public string? DefaultImageUrl { get; set; }
    public List<ProductVariantFeatureValue> ProductVariantFeatureValues { get; set; } = new List<ProductVariantFeatureValue>();
    public List<ProductVariantImages> productVariantImages { get; set; } = new List<ProductVariantImages>();
    public List<ProductVariantMedia> ProductVariantMedia { get; set; } = new List<ProductVariantMedia>();
}
