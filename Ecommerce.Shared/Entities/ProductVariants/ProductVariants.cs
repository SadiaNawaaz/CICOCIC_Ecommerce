using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Colors;
using Ecommerce.Shared.Entities.ModelYears;
using Ecommerce.Shared.Entities.Products;
using Ecommerce.Shared.Entities.Sizes;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Ecommerce.Shared.Entities.ProductVariants;
public class ProductVariant : BaseEntity
{

    public long ProductId { get; set; }
    public Product Product { get; set; }
    [Required]
    public string Name { get; set; }
    public long? GeneralSizeId { get; set; }
    public GeneralSize? GeneralSize { get; set; }
    public long? GeneralColorId { get; set; }
    public GeneralColor? GeneralColor { get; set; }
    public double Price { get; set; }
    public string? Sku { get; set; }
    public string? Value { get; set; }
    public string? SSN { get; set; }
    public string ?Description { get;set; }

    public long? ModelYearId { get; set; }
    public ModelYear? ModelYear { get; set; }
    public bool Publish { get; set; }

}
