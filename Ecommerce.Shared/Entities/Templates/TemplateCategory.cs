

using Ecommerce.Shared.Abstraction;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Entities.Templates;

public class TemplateCategory : BaseEntity
{


    public long TemplateMasterId { get; set; }
    public TemplateMaster TemplateMaster { get; set; }


    public long CategoryId { get; set; }
    public Category Category { get; set; }


}
