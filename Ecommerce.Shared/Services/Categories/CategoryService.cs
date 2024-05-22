using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Ecommerce.Shared.Services.Categories;

public interface ICategoryService
{
    Task<ServiceResponse<List<Category>>> GetCategoriesAsync();
    Task<ServiceResponse<Category>> GetCategoryByIdAsync(long id);
    Task<ServiceResponse<Category>> AddCategoryAsync(Category category);
    Task<ServiceResponse<Category>> UpdateCategoryAsync(Category category);
    Task<ServiceResponse<bool>> DeleteCategoryAsync(long id);
}

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Category>>> GetCategoriesAsync()
    {
        try
        {
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
            // Retrieve the existing category from the database
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
        // Add the current category to the list of categories to delete
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





}




