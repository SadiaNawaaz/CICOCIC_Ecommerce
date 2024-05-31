using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities.Brands;

public class Brand: BaseEntity
{
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
}


