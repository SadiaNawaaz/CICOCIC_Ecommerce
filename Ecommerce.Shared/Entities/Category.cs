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
    public string? Icon { get; set; } = "";
    public long? ParentCategoryId { get; set; } 

    [ForeignKey("ParentCategoryId")]
    public Category? ParentCategory { get; set; }
    public int Order { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();

    [NotMapped]
    public byte[] ImageByte { get; set; }
    public bool MarkCategory { get; set; } = false;

    public ICollection<CategoryTranslation> Translations { get; set; } = new List<CategoryTranslation>();
    }


public class CategoryTranslation: BaseDetailEntity
    {


    [Required]
    public long CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public Category Category { get; set; }

    [Required]
    public int LanguageId { get; set; }
    [ForeignKey("LanguageId")]
    public Language Language { get; set; }

    [Required, MaxLength(255)]
    public string TranslatedName { get; set; } = string.Empty;
    }

public class CategoryVariantCountDto
    {
    public long CategoryId { get; set; } // The ID of the category
    public string CategoryName { get; set; } // The name of the category
    public int VariantCount { get; set; } // The count of all product variants under this category
    }
