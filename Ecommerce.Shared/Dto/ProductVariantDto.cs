

using Ecommerce.Shared.Entities.Colors;
using Ecommerce.Shared.Entities.ModelYears;
using Ecommerce.Shared.Entities.Products;
using Ecommerce.Shared.Entities.ProductVariants;
using Ecommerce.Shared.Entities.Sizes;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Dto;

public class ProductVariantDto
{

    public long Id { get; set; }
    public long ProductId { get; set; }
    public string Name { get; set; }
    public string Category {  get; set; }
    public string Color { get; set; }
    public string Size { get; set; }
    public double ProductPrice { get; set; }
    public double VariantPrice { get; set; }
    public string? Sku { get; set; }
    public string? Value { get; set; }
    public string? SSN { get; set; }
    public string? Description { get; set; }
    public int discountPercentage { get; set; } = 0;
    public bool Publish { get; set; }
    public string ?DefaultImageUrl { get; set; }
    public List<ProductVariantFeatureValue> ProductVariantFeatureValues { get; set; } = new List<ProductVariantFeatureValue>();
    public List<ProductVariantImages> productVariantImages { get; set; } = new List<ProductVariantImages>();
}
