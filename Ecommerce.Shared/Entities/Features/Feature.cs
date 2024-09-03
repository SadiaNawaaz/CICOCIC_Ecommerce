

using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Clusters;
using Ecommerce.Shared.Entities.ModelYears;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ecommerce.Shared.Entities.Features;

public class Feature : BaseEntity
{
    [Required]
    public string Name { get; set; }  
    public long ClusterId { get; set; }
    public Cluster Cluster { get; set; }
    [NotMapped]
    public string Value { get; set; }
}
