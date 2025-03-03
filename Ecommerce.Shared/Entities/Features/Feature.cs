

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
    public List<FeatureTranslation> Translations { get; set; } = new List<FeatureTranslation>();
    }
public class FeatureTranslation: BaseDetailEntity
    {


    [Required]
    public long FeatureId { get; set; }

    [ForeignKey("FeatureId")]
    public Feature Feature { get; set; }

    [Required]
    public int LanguageId { get; set; }

   [ForeignKey("LanguageId")]
   public Language Language { get; set; }

    [Required, MaxLength(255)]
    public string TranslatedName { get; set; } = string.Empty;
    }
