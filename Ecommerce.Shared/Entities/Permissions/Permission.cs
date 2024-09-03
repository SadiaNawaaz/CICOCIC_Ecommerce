using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities.Permissions;

public class Permission
{
  
    public long Id { get; set; }
    public string ActionName { get; set; } 
    public string? PageUrl { get; set; }
    public string ?Description { get; set; }
    public long ParentId { get; set; }
}
public class PermissionDto
{
    public long Id { get; set; }
    public string ActionName { get; set; }
    public string? PageUrl { get; set; }
    public string? Description { get; set; }
    public long ParentId { get; set; }
    public bool Checked { get; set; } = false;
}
