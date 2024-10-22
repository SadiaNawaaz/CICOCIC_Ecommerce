using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Dto;

public class CartItemDto
{
    public long VariantId { get; set; }
    public long ProductId { get; set; }
    public string VariantName { get; set; }
    public int  Qty { get; set; }
    public double VariantPrice { get; set; }
    public double ProducrPrice { get; set; }
    public string?  ImageName { get; set; }
    public string? Color { get; set; }
    public string? Thumbnail { get; set; }
}
