using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Entities.Catalogs;
using Ecommerce.Shared.Entities.Clusters;
using Ecommerce.Shared.Entities.Features;
using Ecommerce.Shared.Entities.ProductVariants;
using Ecommerce.Shared.Entities.Templates;
using Ecommerce.Shared.Enums;
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
    public long? CatalogId { get; set; }
    public Catalog? Catalog { get; set; }
    public long CategoryId { get; set; }
    public Category Category { get; set; }
    [Required]
    public long BrandId { get; set; }
    public Brand Brand { get; set; }
    public double Price { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? EanNumber { get; set; }
    public List<ProductCluster> ProductClusters { get; set; } = new List<ProductCluster>();
    public List<ProductImages> ProductImages { get; set; } = new List<ProductImages>();
    public List<ProductMedia> ProductMedias { get; set; } = new List<ProductMedia>();
    public List<ProductTranslation> Translations { get; set; } = new List<ProductTranslation>();
    

    }

public class ProductCluster : BaseEntity
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public long ClusterId { get; set; }
    public Cluster Cluster { get; set; }
    public int Order { get; set; }
    public List<ProductClusterFeature> ProductClusterFeatures { get; set; } = new List<ProductClusterFeature>();
    [NotMapped]
    public bool IsDragOver { get; set; }

}

public class ProductClusterFeature : BaseEntity
{
    public long ProductClusterId { get; set; }
    public ProductCluster ProductCluster { get; set; }
    public long FeatureId { get; set; }
    public Feature Feature { get; set; }
    public string? Value { get; set; }
}

public class ProductImages : BaseEntity
{

    public long ProductId { get; set; }
    public Product Product { get; set; }
    [NotMapped]
    public byte[] ImageByte { get; set; }
    [NotMapped]
    public string ImageUrl { get; set; }
    public string ImageName { get; set; }

    [NotMapped]
    public bool IsDeleted { get; set; } = false;

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


public class ProductMedia
{
    public long Id { get; set; }

    [Required]
    public string MediaUrl { get; set; }

    public long ProductId { get; set; }
    public Product Product { get; set; }

    public string ContentType  { get; set; }
    }




public class ProductTranslation :BaseDetailEntity
    {
    [Required]
    public long ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    [Required]
    public int LanguageId { get; set; }

    [ForeignKey("LanguageId")]
    public Language Language { get; set; }

    [Required, MaxLength(255)]
    public string TranslatedName { get; set; } = string.Empty;

    public string? TranslatedDescription { get; set; }
    public string? TranslatedShortDescription { get; set; }
    }
