using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Dto;

public class OrderDto
    {
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public double TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
    }

public class OrderItemDto
    {
    public long Id { get; set; }
    public string ProductName { get; set; }
    public string VariantName { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string? DefaultImageUrl { get; set; }
    }




