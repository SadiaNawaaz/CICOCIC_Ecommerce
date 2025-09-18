using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities.Orders;

public class Order
    {
    public long Id { get; set; }
    public string OrderNumber { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? CompanyName { get; set; }
    public string AreaCode { get; set; }
    public string PrimaryPhone { get; set; }
    public string StreetAddress { get; set; }
    public string? ZipCode { get; set; }
    public bool IsBusinessAddress { get; set; }
    public double TotalAmount { get; set; }
    public DateTime OrderDate {  get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

public class OrderItem
    {
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    public long VariantId { get; set; }
    public string ProductName { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    }


