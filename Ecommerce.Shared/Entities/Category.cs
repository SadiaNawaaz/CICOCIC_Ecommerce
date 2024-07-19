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
    public string? Icon { get; set; } = "Icons.Material.Filled.Folder";
    public long? ParentCategoryId { get; set; } 

    [ForeignKey("ParentCategoryId")]
    public Category? ParentCategory { get; set; }
    public int Order { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
}
