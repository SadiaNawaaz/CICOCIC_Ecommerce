using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Dto;

public class TrendingProductDto
{
    public long Id { get; set; }
   public long ProductVariantId { get; set; }
    public long ProductId { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
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
    public string? DefaultImageUrl { get; set; }
}
