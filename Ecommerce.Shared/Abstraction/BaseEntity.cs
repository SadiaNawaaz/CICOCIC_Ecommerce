

using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Abstraction;

public abstract class BaseEntity
{
    [Key]
    public long Id { get; set; }    
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
