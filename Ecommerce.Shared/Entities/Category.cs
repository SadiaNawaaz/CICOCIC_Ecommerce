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

    [NotMapped]
    public byte[] ImageByte { get; set; }
}
public class CategoryVariantCountDto
    {
    public long CategoryId { get; set; } // The ID of the category
    public string CategoryName { get; set; } // The name of the category
    public int VariantCount { get; set; } // The count of all product variants under this category
    }
