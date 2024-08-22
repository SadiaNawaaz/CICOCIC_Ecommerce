using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Features;
using Ecommerce.Shared.Entities.Products;
using System.ComponentModel.DataAnnotations;
namespace Ecommerce.Shared.Entities.Clusters;

public class Cluster:BaseEntity
{

    [Required]
    public string Name { get; set; }
    public List<Feature>  Features { get; set; } = new List<Feature>();
}
