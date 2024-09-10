

using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Roles;
using Ecommerce.Shared.Services.Roles;
using System.ComponentModel.DataAnnotations;
namespace Ecommerce.Shared.Entities;

public class User: BaseEntity
{
    [Required]
    public string FirstName { get; set; }

    public string LastName { get; set; } = "";
    [Required]
    public string UserName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string? ExtraEmail { get; set; }
    [Required]
    [StringLength(100)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
 
    public bool IsAgent { get; set; } = false;
    public bool? Approved { get; set; } = false;

    public string? PostalCode { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();



}
