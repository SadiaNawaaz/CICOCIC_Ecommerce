﻿using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Entities.Clusters;
using Ecommerce.Shared.Entities.Colors;
using Ecommerce.Shared.Entities.Features;
using Ecommerce.Shared.Entities.ModelYears;
using Ecommerce.Shared.Entities.Products;
using Ecommerce.Shared.Entities.Sizes;
using Ecommerce.Shared.Entities.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities.Catalogs;

public class Catalog : BaseEntity
{
    [Required]
    public  string Name { get; set; }
    public  string? Slug { get; set; }
    public long Code { get; set; }
    public string? Thumbnail { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }



    public long CategoryId { get; set; }
    public Category Category { get; set; }
    public long? BrandId { get; set; }
    public Brand Brand { get; set; }
    public long? GeneralSizeId { get; set; }
    public GeneralSize? GeneralSize { get; set; }
    public long? GeneralColorId { get; set; }
    public GeneralColor? GeneralColor { get; set; }
    public long? ModelYearId { get; set; }
    public ModelYear? ModelYear { get; set; }
    public string Media { get; set; } = "[]";

    public List<CatalogCluster> CatalogClusters { get; set; } = new List<CatalogCluster>();
}
public class CatalogCluster : BaseEntity
{
    public long CatalogId { get; set; }
    public Catalog Catalog { get; set; }

    public long ClusterId { get; set; }
    public Cluster Cluster { get; set; }
    public List<CatalogClusterFeature> CatalogClusterFeatures { get; set; } = new List<CatalogClusterFeature>();


}

public class CatalogClusterFeature : BaseEntity
{
    public long CatalogClusterId { get; set; }
    public CatalogCluster CatalogCluster { get; set; }

    public long FeatureId { get; set; }
    public Feature Feature { get; set; }

    public string? Value { get; set; }




}