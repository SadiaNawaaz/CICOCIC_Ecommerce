using Ecommerce.Shared.Abstraction;
using System.ComponentModel.DataAnnotations;
namespace Ecommerce.Shared.Entities.Clusters;

public class Cluster:BaseEntity
{

    [Required]
    public string Name { get; set; }
}
