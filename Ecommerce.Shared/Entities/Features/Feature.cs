

using Ecommerce.Shared.Abstraction;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ecommerce.Shared.Entities.Features;

public class Feature : BaseEntity
{
    public string Name { get; set; }
    [NotMapped]
    public string Value { get; set; }
}
