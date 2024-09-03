

using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Abstraction;

public abstract class BaseEntity
{
    [Key]
    public long Id { get; set; }    
    public long? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }= DateTime.Now;
    public long? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; } = DateTime.Now;
}
