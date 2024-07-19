using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Products;
using Ecommerce.Shared.Entities.ProductVariants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities.TrendingProducts;

public class TrendingProduct : BaseEntity
{
    public long ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; }
}
