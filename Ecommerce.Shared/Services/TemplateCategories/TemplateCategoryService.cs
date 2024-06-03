
using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Templates;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.TemplateCategories;

public interface ITemplateCategoryService
{
    Task<ServiceResponse<List<TemplateCategory>>> GetTemplateCategoriesAsync();
    Task<ServiceResponse<TemplateCategory>> GetTemplateCategoryByIdAsync(long id);
    Task<ServiceResponse<TemplateCategory>> AddTemplateCategoryAsync(TemplateCategory templateCategory);
    Task<ServiceResponse<TemplateCategory>> UpdateTemplateCategoryAsync(TemplateCategory templateCategory);
    Task<ServiceResponse<bool>> DeleteTemplateCategoryAsync(long id);
    Task<TemplateCategory> GetTemplateCategoryByCategoryIdAsync(long categoryId);
}
public class TemplateCategoryService : ITemplateCategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TemplateCategoryService> _logger;

    public TemplateCategoryService(ApplicationDbContext context, ILogger<TemplateCategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<TemplateCategory>>> GetTemplateCategoriesAsync()
    {
        try
        {
            var templateCategories = await _context.TemplateCategories
                .Include(tc => tc.TemplateMaster)
                .Include(tc => tc.Category)
                .ToListAsync();

            return new ServiceResponse<List<TemplateCategory>>
            {
                Data = templateCategories,
                Success = true,
                Message = "Template categories fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching template categories.");
            return new ServiceResponse<List<TemplateCategory>>
            {
                Success = false,
                Message = "Error occurred while fetching template categories"
            };
        }
    }

    public async Task<ServiceResponse<TemplateCategory>> GetTemplateCategoryByIdAsync(long id)
    {
        try
        {
            var templateCategory = await _context.TemplateCategories
                .Include(tc => tc.TemplateMaster)
                .Include(tc => tc.Category)
                .FirstOrDefaultAsync(tc => tc.Id == id);

            if (templateCategory == null)
            {
                return new ServiceResponse<TemplateCategory>
                {
                    Success = false,
                    Message = "Template category not found"
                };
            }

            return new ServiceResponse<TemplateCategory>
            {
                Data = templateCategory,
                Success = true,
                Message = "Template category fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching template category by ID.");
            return new ServiceResponse<TemplateCategory>
            {
                Success = false,
                Message = "Error occurred while fetching template category by ID"
            };
        }
    }

    public async Task<ServiceResponse<TemplateCategory>> AddTemplateCategoryAsync(TemplateCategory templateCategory)
    {
        try
        {
            _context.TemplateCategories.Add(templateCategory);
            await _context.SaveChangesAsync();
            return new ServiceResponse<TemplateCategory>
            {
                Data = templateCategory,
                Success = true,
                Message = "Template category added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding template category.");
            return new ServiceResponse<TemplateCategory>
            {
                Success = false,
                Message = "Error occurred while adding template category"
            };
        }
    }

    public async Task<ServiceResponse<TemplateCategory>> UpdateTemplateCategoryAsync(TemplateCategory updatedTemplateCategory)
    {
        try
        {
            var existingTemplateCategory = await _context.TemplateCategories.FindAsync(updatedTemplateCategory.Id);

            if (existingTemplateCategory == null)
            {
                return new ServiceResponse<TemplateCategory>
                {
                    Success = false,
                    Message = "Template category not found"
                };
            }

            existingTemplateCategory.TemplateMasterId = updatedTemplateCategory.TemplateMasterId;
            existingTemplateCategory.CategoryId = updatedTemplateCategory.CategoryId;

            _context.TemplateCategories.Update(existingTemplateCategory);
            await _context.SaveChangesAsync();

            return new ServiceResponse<TemplateCategory>
            {
                Data = existingTemplateCategory,
                Success = true,
                Message = "Template category updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating template category.");
            return new ServiceResponse<TemplateCategory>
            {
                Success = false,
                Message = "Error occurred while updating template category"
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteTemplateCategoryAsync(long id)
    {
        try
        {
            var templateCategory = await _context.TemplateCategories.FindAsync(id);

            if (templateCategory == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Template category not found"
                };
            }

            _context.TemplateCategories.Remove(templateCategory);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Template category deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting template category.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting template category"
            };
        }
    }

    public async Task<TemplateCategory> GetTemplateCategoryByCategoryIdAsync(long categoryId)
    {
        try
        {
            var templateCategory = await _context.TemplateCategories
                .FirstOrDefaultAsync(tc => tc.CategoryId == categoryId);

            return templateCategory;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching template category by CategoryId.");
            throw; // Handle the exception appropriately
        }
    }

}

