using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Permissions;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Entities.Roles;

public class Role : BaseEntity
{
    [Required]
    public  string Name { get; set; }
    public string? Description { get; set; }
    public List<RolePermission> RolePermissions { get; set; }
}

public class RolePermission : BaseEntity
{
    public long RoleId { get; set; }
    public Role Role { get; set; }
    public long PermissionId { get; set; }
    public Permission Permission { get; set; }
}
