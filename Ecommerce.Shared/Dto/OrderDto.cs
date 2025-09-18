using Ecommerce.Shared.Enums;
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
    public string Email { get; set; }
    public double TotalAmount { get; set; }
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public string AreaCode { get; set; }
    public string PrimaryPhone { get; set; }
    public string StreetAddress { get; set; }
    public OrderStatus PaymentStatus { get; set; }         
    public int TotalItems => OrderItems?.Sum(x => x.Quantity) ?? 0;
    public string DeliveryMethod { get; set; }
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




