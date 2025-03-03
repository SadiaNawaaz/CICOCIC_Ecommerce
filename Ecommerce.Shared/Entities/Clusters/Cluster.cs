using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Features;
using Ecommerce.Shared.Entities.Products;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ecommerce.Shared.Entities.Clusters;

public class Cluster:BaseEntity
{

    [Required]
    public string Name { get; set; }
    public List<Feature>  Features { get; set; } = new List<Feature>();
    public List<ClusterTranslation> Translations { get; set; } = new List<ClusterTranslation>();
    }
public class ClusterTranslation : BaseDetailEntity
    {


    [Required]
    public long ClusterId { get; set; }

    [ForeignKey("ClusterId")]
    public Cluster Cluster { get; set; }
    
    [Required]
    public int LanguageId { get; set; }

    [ForeignKey("LanguageId")]
   public Language Language { get; set; }

    [Required, MaxLength(255)]
    public string TranslatedName { get; set; } = string.Empty;
    }
