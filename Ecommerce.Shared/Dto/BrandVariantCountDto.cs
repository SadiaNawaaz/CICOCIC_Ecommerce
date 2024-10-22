using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Dto;

public class BrandVariantCountDto
    {
    public long BrandId { get; set; }
    public string BrandName { get; set; }
    public int VariantCount { get; set; }
    }

