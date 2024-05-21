using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Ecommerce.Shared.Abstraction;

namespace Ecommerce.Shared.Entities;

public class Category: BaseEntity
{
    [Required]
    public string Name { get; set; }
    [Required]
    public int Level { get; set; }
    public long? ParentCategoryId { get; set; } 

    [ForeignKey("ParentCategoryId")]
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
}
