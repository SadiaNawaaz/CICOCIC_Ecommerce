using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities;

public class IntegrationUserLink : BaseEntity
    {
    public int ExternalUserId { get; set; } 
    public string? Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public long EcommerceUserId { get; set; }
    public long ProductId { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }

