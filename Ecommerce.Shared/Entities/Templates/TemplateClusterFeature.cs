using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities.Templates;

public class TemplateClusterFeature: BaseEntity
{
    public long FeatureId { get; set; }
    public Feature Feature { get; set; }
    public long TemplateClusterId { get; set; }  
    public TemplateCluster TemplateCluster { get; set; } 
}
