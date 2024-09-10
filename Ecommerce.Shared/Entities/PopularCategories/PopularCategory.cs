using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities.PopularCategories;

public class PopularCategory : BaseEntity
{
    [ForeignKey("Category")]
    public long CategoryId { get; set; }

    [Required]
    public string BannerUrl { get; set; }

    public int Order { get; set; }

    public Category Category { get; set; }

    [NotMapped]
    public byte[] ImageByte { get; set; }
}
public class PopularCategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string BannerUrl { get; set; }
    public long CategoryId { get; set; }
    public int Order { get; set; }
}

