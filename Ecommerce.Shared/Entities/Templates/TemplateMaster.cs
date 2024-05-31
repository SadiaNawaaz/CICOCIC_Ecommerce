

using Ecommerce.Shared.Abstraction;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Entities.Templates;

public class TemplateMaster:BaseEntity
{

    [Required]
    public string Name { get; set; }
    public List<TemplateCluster> Clusters { get; set; } = new List<TemplateCluster>();

}
