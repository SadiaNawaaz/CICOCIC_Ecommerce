using Ecommerce.Shared.Abstraction;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Entities.Roles;

public class Role : BaseEntity
{
    [Required]
    public  string Name { get; set; }
    public string? Description { get; set; }
}
