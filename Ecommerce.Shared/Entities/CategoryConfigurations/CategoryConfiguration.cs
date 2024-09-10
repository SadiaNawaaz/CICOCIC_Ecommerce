using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities.CategoryConfigurations;

public class CategoryConfiguration : BaseEntity
{
    [ForeignKey("Category")]
    public long CategoryId { get; set; }

    [Required]
    public string BannerUrl { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public decimal StartingFromPrice { get; set; }

    public Category Category { get; set; }

    [NotMapped]
    public byte[] ImageByte { get; set; }
}
public class CategoryConfigurationDto
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string BannerUrl { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal StartingFromPrice { get; set; }
}

