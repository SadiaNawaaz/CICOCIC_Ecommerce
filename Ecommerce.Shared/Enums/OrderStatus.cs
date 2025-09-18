using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Enums;

public enum OrderStatus
    {
    Pending,       // Order created but not paid yet
    Paid,          // Payment successful
    Failed,        // Payment failed
    Cancelled,     // User cancelled or order timed out
    Shipped,       // Order shipped
    Delivered,     // Delivered to customer
    Refunded       // Payment refunded
    }