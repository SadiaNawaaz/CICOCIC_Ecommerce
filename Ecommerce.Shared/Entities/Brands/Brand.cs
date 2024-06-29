using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities.Brands;

public class Brand: BaseEntity
{
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailFileUrl { get; set; }
    [NotMapped]
    public byte[] ImageByte { get; set; }
}


