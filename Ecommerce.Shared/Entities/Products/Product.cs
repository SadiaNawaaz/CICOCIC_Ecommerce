using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Entities.Features;
using Ecommerce.Shared.Entities.Templates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities.Products;

public class Product : BaseEntity
{
    [Required]
    public string Name { get; set; }
    public string? Slug { get; set; }
    public string? Code { get; set; }

    public string? Thumbnail { get; set; }   
    public long CategoryId { get; set; }
    public Category Category { get; set; }
    public long TemplateMasterId { get; set; }
    public TemplateMaster TemplateMaster { get; set; }
    [Required]
    public long BrandId { get; set; }
    public Brand Brand { get; set; }
    public double Price { get; set; }
    public List<ProductFeatureValue> FeatureValues { get; set; } = new List<ProductFeatureValue>();


}
public class ProductFeatureValue : BaseEntity
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    [ForeignKey("TemplateClusterFeatureId")]
    public long TemplateClusterFeatureId { get; set; }
    public TemplateClusterFeature TemplateClusterFeature { get; set; }
    public string? Value { get; set; }
}