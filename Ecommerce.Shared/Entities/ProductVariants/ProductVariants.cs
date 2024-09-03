using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Colors;
using Ecommerce.Shared.Entities.ModelYears;
using Ecommerce.Shared.Entities.Products;
using Ecommerce.Shared.Entities.Sizes;
using Ecommerce.Shared.Entities.Templates;
using Ecommerce.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    public string? Thumbnail { get; set; }
    public long? ModelYearId { get; set; }
    public ModelYear? ModelYear { get; set; }
    public bool Publish { get; set; }
   public VariantType? variantType { get; set; }
    public string? TypeValue { get; set; }
    public string? Note { get; set; }
    public List<ProductVariantImages> productVariantImages { get; set; } = new List<ProductVariantImages>();
    public List<ProductVariantMedia> ProductVariantMedias { get; set; } = new List<ProductVariantMedia>();





}
public class ProductVariantFeatureValue : BaseEntity
{

    public long ProductVariantId { get; set; }
    public virtual ProductVariant ProductVariant { get; set; }

    [ForeignKey("TemplateClusterFeatureId")]
    public long TemplateClusterFeatureId { get; set; }
    public TemplateClusterFeature TemplateClusterFeature { get; set; }
    public string? Value { get; set; }
}


public class ProductVariantImages :BaseEntity
    {

    public long ProductVariantId { get; set; }
    public virtual ProductVariant ProductVariant { get; set; }
    [NotMapped]
    public byte[] ImageByte { get; set; }
    [NotMapped]
    public string ImageUrl { get; set; }
    public string ImageName { get; set; }
    public int Order { get; set; } 

    [NotMapped]
    public bool IsDeleted { get; set; } = false;

    [NotMapped]
    public bool IsCloned { get; set; } = false;



}


public class ProductVariantMedia
{
    public long Id { get; set; }

    [Required]
    public string ImageUrl { get; set; }

    public long ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; }
}
