﻿

namespace Ecommerce.Shared.Dto;

public class CategoryDto
{
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Level { get; set; }
        public long? ParentCategoryId { get; set; }
        public HashSet<CategoryDto> SubCategories { get; set; } = new HashSet<CategoryDto>();
        public int Order { get; set; }
        public bool IsDragOver { get; set; }
        public string? IconPath { get; set; } = "";
        public byte[] ImageByte { get; set; } = null;



}
