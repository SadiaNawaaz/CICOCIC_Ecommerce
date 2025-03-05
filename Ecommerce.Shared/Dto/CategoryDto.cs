

using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Dto;

public class CategoryDto
{
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Level { get; set; }
        public long? ParentCategoryId { get; set; }
        public List<CategoryDto> SubCategories { get; set; } = new List<CategoryDto>();
        public int Order { get; set; }
        public bool IsDragOver { get; set; }
        public string? IconPath { get; set; } = "";
        public byte[] ImageByte { get; set; } = null;
    public bool IsExpanded { get; set; } = false;
    public bool HasChild { get; set; } = true;
    public bool MarkCategory { get; set; } = false;
    public ICollection<CategoryTranslationDto> Translations { get; set; } = new List<CategoryTranslationDto>();



}
public class CategoryTranslationDto 
    {


    public long Id { get; set; }
    public long CategoryId { get; set; }
    public Category Category { get; set; }
    public int LanguageId { get; set; }
    public string TranslatedName { get; set; } = string.Empty;
    }