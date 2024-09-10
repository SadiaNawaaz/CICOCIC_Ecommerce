using Ecommerce.Shared.Entities.Brands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Shared.Abstraction;

namespace Ecommerce.Shared.Entities.PopularBrands;

public class PopularBrand : BaseEntity
{
  

    [ForeignKey("Brand")]
    public long BrandId { get; set; }

    [Required]
    public string BannerUrl { get; set; }

    public int order {  get; set; }

    public Brand Brand { get; set; }

    [NotMapped]
    public byte[] ImageByte { get; set; }
}
public class PopularBrandDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string BannerUrl { get; set; }
    public long BrandId { get; set; }
    public int Order { get; set; }
}