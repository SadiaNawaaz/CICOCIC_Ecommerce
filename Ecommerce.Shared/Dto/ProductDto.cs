using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Entities.Templates;
using Ecommerce.Shared.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Dto;

public class ProductDto
{

    public long Id { get; set; }    
    public string Name { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public double Price { get; set; }
    public string? Thumbnail { get; set; }
    public int Stock { get; set; }
  
}
