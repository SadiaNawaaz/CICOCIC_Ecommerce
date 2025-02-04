using Ecommerce.Shared.Context;
using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
namespace Ecommerce.Shared.Services.Categories;

public interface ICategoryService
{
    Task<ServiceResponse<List<Category>>> GetCategoriesAsync();
    Task<ServiceResponse<Category>> GetCategoryByIdAsync(long id);
    Task<ServiceResponse<Category>> AddCategoryAsync(Category category);
    Task<ServiceResponse<Category>> UpdateCategoryAsync(Category category);
    Task<ServiceResponse<bool>> DeleteCategoryAsync(long id);
    Task<ServiceResponse<List<CategoryDto>>> GetCategoriesDtoAsync();
    Task<ServiceResponse<List<CategoryDto>>> GetCategoriesTreeDtoAsync();
    Task<ServiceResponse<List<CategoryDto>>> GetCategoriesTopLevelDtoAsync();
    Task<ServiceResponse<bool>> UpdateCategoryOrderAsync(long categoryId, int newOrder);
    Task<ServiceResponse<List<CategoryDto>>> GetCategoryHierarchyAsync(long categoryId);
    Task<ServiceResponse<bool>> ImportCategoriesAsync(List<Category> icecatCategories);

}

public class CategoryService : ICategoryService
{
    
    private readonly ILogger<CategoryService> _logger;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public CategoryService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<CategoryService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Category>>> GetCategoriesAsync()
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var categories = await _context.Categories.Include(c => c.SubCategories).ToListAsync();
            return new ServiceResponse<List<Category>>
            {
                Data = categories,
                Success = true,
                Message = "Categories fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching categories.");
            return new ServiceResponse<List<Category>>
            {
                Success = false,
                Message = "Error occurred while fetching categories"
            };
        }
    }

    public async Task<ServiceResponse<Category>> GetCategoryByIdAsync(long id)
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var category = await _context.Categories.Include(c => c.SubCategories).FirstOrDefaultAsync(c => c.Id == id);
            return new ServiceResponse<Category>
            {
                Data = category,
                Success = true,
                Message = "Category fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching category by ID.");
            return new ServiceResponse<Category>
            {
                Success = false,
                Message = "Error occurred while fetching category by ID"
            };
        }
    }

    public async Task<ServiceResponse<Category>> AddCategoryAsync(Category category)
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return new ServiceResponse<Category>
            {
                Data = category,
                Success = true,
                Message = "Category added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding category.");
            return new ServiceResponse<Category>
            {
                Success = false,
                Message = "Error occurred while adding category"
            };
        }
    }

    public async Task<ServiceResponse<Category>> UpdateCategoryAsync(Category updatedCategory)
    {
        try
        {
            using var _context = _contextFactory.CreateDbContext();
            var existingCategory = await _context.Categories.FindAsync(updatedCategory.Id);

            if (existingCategory == null)
            {
                return new ServiceResponse<Category>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            existingCategory.Name = updatedCategory.Name;
            existingCategory.Icon = updatedCategory.Icon;
            existingCategory.Level = updatedCategory.Level;
            existingCategory.ParentCategoryId = updatedCategory.ParentCategoryId;
            existingCategory.LastModifiedDate = DateTime.Now;
            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Category>
            {
                Data = existingCategory,
                Success = true,
                Message = "Category updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating category.");
            return new ServiceResponse<Category>
            {
                Success = false,
                Message = "Error occurred while updating category"
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteCategoryAsync(long id)
    {
        try
        {
            using var _context = _contextFactory.CreateDbContext();
            using (var transaction = _context.Database.BeginTransaction())
            {
                var categoriesToDelete = new List<Category>();
                await GetCategoriesToDeleteRecursive(id, categoriesToDelete);

                if (!categoriesToDelete.Any())
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Category not found"
                    };
                }

                _context.Categories.RemoveRange(categoriesToDelete);
                await _context.SaveChangesAsync();

                transaction.Commit();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Category and all its subcategories deleted successfully"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting category.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting category"
            };
        }
    }

    private async Task GetCategoriesToDeleteRecursive(long id, List<Category> categoriesToDelete)
    {
        using var _context = _contextFactory.CreateDbContext();
        var categoryToDelete = await _context.Categories.FindAsync(id);
        if (categoryToDelete != null)
        {
            categoriesToDelete.Add(categoryToDelete);

            // Fetch the child categories recursively
            var childCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == id)
                .ToListAsync();

            foreach (var childCategory in childCategories)
            {
                await GetCategoriesToDeleteRecursive(childCategory.Id, categoriesToDelete);
            }
        }
    }


    public async Task<ServiceResponse<List<CategoryDto>>> GetCategoriesDtoAsync()
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var categories = await _context.Categories.Include(c => c.SubCategories).ToListAsync();
            var categoryDtos = categories.Select(ToDto).ToList();

            return new ServiceResponse<List<CategoryDto>>
            {
                Data = categoryDtos,
                Success = true,
                Message = "Categories fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching categories.");
            return new ServiceResponse<List<CategoryDto>>
            {
                Success = false,
                Message = "Error occurred while fetching categories"
            };
        }
    }

    public async Task<ServiceResponse<List<CategoryDto>>> GetCategoriesTreeDtoAsync()
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var categories = await _context.Categories.Where(a=>a.Level>0 && a.Level<6).OrderBy(a => a.Level).ToListAsync();
            var categoryDtos = categories.Select(ToDto).ToList();
            var categoryTree = await BuildCategoryTree(categoryDtos);
            return new ServiceResponse<List<CategoryDto>>
            {
                Data = categoryTree.OrderBy(a=>a.Order).ToList(),
                Success = true,
                Message = "Categories fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching categories.");
            return new ServiceResponse<List<CategoryDto>>
            {
                Success = false,
                Message = "Error occurred while fetching categories"
            };
        }
    }


    public async Task<ServiceResponse<List<CategoryDto>>> GetCategoriesTopLevelDtoAsync()
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var categories = await _context.Categories.Where(a=>a.Level==1).OrderBy(a=>a.Order).ToListAsync();
            var categoryDtos = categories.Select(ToDto).ToList();

            return new ServiceResponse<List<CategoryDto>>
            {
                Data = categoryDtos,
                Success = true,
                Message = "Categories fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching categories.");
            return new ServiceResponse<List<CategoryDto>>
            {
                Success = false,
                Message = "Error occurred while fetching categories"
            };
        }
    }


    public static CategoryDto ToDto(Category category)
    {
        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Icon = category.Icon,
            Level = category.Level,
            Order=category.Order,
            ParentCategoryId = category.ParentCategoryId

        };
    }
    private async Task<List<CategoryDto>> BuildCategoryTree(List<CategoryDto> categories)
    {
        var lookup = categories.ToLookup(c => c.ParentCategoryId);
        foreach (var category in categories)
        {
            if (category.ParentCategoryId != null)
            {
                var parent = categories.FirstOrDefault(c => c.Id == category.ParentCategoryId);
                parent?.SubCategories.Add(category);
            }
        }

        // Return only the root categories (those with no parent)
        return categories.Where(c => c.ParentCategoryId == null).ToList();
    }


    public async Task<ServiceResponse<bool>> UpdateCategoryOrderAsync(long categoryId, int newOrder)
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            category.Order = newOrder;  // Assuming 'Order' is a property of Category
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Category order updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating category order.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while updating category order"
            };
        }
    }

    public async Task<ServiceResponse<List<CategoryDto>>> GetCategoryHierarchyAsync(long categoryId)
        {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var categoryDtos = new List<CategoryDto>();
            var category = await _context.Categories.FindAsync(categoryId);
            while (category != null)
                {
                categoryDtos.Insert(0, new CategoryDto
                    {
                    Id = category.Id,
                    Name = category.Name
                    });
                if (category.ParentCategoryId != null)
                    {
                    category = await _context.Categories.FindAsync(category.ParentCategoryId);
                    }
                else
                    {
                    category = null;
                    }
                }

            return new ServiceResponse<List<CategoryDto>>
                {
                Data = categoryDtos,
                Success = true,
                Message = "Category hierarchy fetched successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching category hierarchy.");
            return new ServiceResponse<List<CategoryDto>>
                {
                Success = false,
                Message = "Error occurred while fetching category hierarchy"
                };
            }
        }


    public async Task<ServiceResponse<bool>> ImportCategoriesAsync(List<Category> icecatCategories)
        {
        using var _context = _contextFactory.CreateDbContext();
        using (var transaction = _context.Database.BeginTransaction())
            {
            try
                {
                // Enable IDENTITY_INSERT for Categories
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Categories ON");

                foreach (var icecatCategory in icecatCategories)
                    {

                    if(icecatCategory.Name==null)
                        {
                        icecatCategory.Name = "";
                        }
                    var existingCategory = await _context.Categories
                        .AsNoTracking() // Add AsNoTracking to avoid tracking issues
                        .FirstOrDefaultAsync(c => c.Id == icecatCategory.Id);

                    if (existingCategory == null)
                        {
                        // New category
                        _context.Categories.Add(icecatCategory);
                        }
                    else
                        {
                        // Existing category, update details
                        _context.Entry(existingCategory).State = EntityState.Detached; // Detach if already tracked
                        _context.Update(icecatCategory);
                        }
                    }

                await _context.SaveChangesAsync();

                // Disable IDENTITY_INSERT for Categories
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Categories OFF");

                transaction.Commit(); // Commit the transaction after all operations

                return new ServiceResponse<bool>
                    {
                    Data = true,
                    Success = true,
                    Message = "Categories imported successfully"
                    };
                }
            catch (Exception ex)
                {
                // Roll back the transaction if any errors occur
                transaction.Rollback();
                _logger.LogError(ex, "Error occurred while importing categories.");
                return new ServiceResponse<bool>
                    {
                    Success = false,
                    Message = "Error occurred while importing categories: " + ex.Message
                    };
                }
            }
        }


    }



