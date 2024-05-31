

using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Clusters;

namespace Ecommerce.Shared.Entities.Templates;

public class TemplateCluster : BaseEntity
{

    public long TemplateMasterId { get; set; }  
    public TemplateMaster TemplateMaster { get; set; }  
    public long ClusterId { get; set; }
    public Cluster cluster { get; set; }
    public List<TemplateClusterFeature> Features { get; set; } = new List<TemplateClusterFeature>();

}
