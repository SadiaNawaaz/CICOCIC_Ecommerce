using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.CategoryConfigurations;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.CategoryConfigurations;

public interface ICategoryConfigurationService
{
    Task<ServiceResponse<List<CategoryConfigurationDto>>> GetCategoryConfigurationsAsync();
    Task<ServiceResponse<CategoryConfigurationDto>> GetCategoryConfigurationByCategoryIdAsync(long categoryId);
    Task<ServiceResponse<bool>> SaveCategoryConfigurationAsync(CategoryConfiguration configuration, byte[] bannerImage, string fileName);
    Task<ServiceResponse<bool>> DeleteCategoryConfigurationAsync(long configurationId);
    Task<ServiceResponse<CategoryConfigurationDto>> GetLastInsertedConfigurationAsync(); // New method
}
public class CategoryConfigurationService : ICategoryConfigurationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoryConfigurationService> _logger;

    public CategoryConfigurationService(ApplicationDbContext context, ILogger<CategoryConfigurationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<CategoryConfigurationDto>>> GetCategoryConfigurationsAsync()
    {
        try
        {
            var configurations = await _context.CategoryConfigurations
                .Include(cc => cc.Category)
                .Select(cc => new CategoryConfigurationDto
                {
                    Id = cc.Id,
                    CategoryId = cc.CategoryId,
                    CategoryName = cc.Category.Name,
                    BannerUrl = cc.BannerUrl,
                    Title = cc.Title,
                    Description = cc.Description,
                    StartingFromPrice = cc.StartingFromPrice
                })
                .ToListAsync();

            return new ServiceResponse<List<CategoryConfigurationDto>>
            {
                Data = configurations,
                Success = true,
                Message = "Configurations loaded successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configurations.");
            return new ServiceResponse<List<CategoryConfigurationDto>>
            {
                Success = false,
                Message = "Error loading configurations."
            };
        }
    }

    public async Task<ServiceResponse<CategoryConfigurationDto>> GetCategoryConfigurationByCategoryIdAsync(long categoryId)
    {
        try
        {
            var configuration = await _context.CategoryConfigurations
                .Where(cc => cc.CategoryId == categoryId)
                .Select(cc => new CategoryConfigurationDto
                {
                    Id = cc.Id,
                    CategoryId = cc.CategoryId,
                    CategoryName = cc.Category.Name,
                    BannerUrl = cc.BannerUrl,
                    Title = cc.Title,
                    Description = cc.Description,
                    StartingFromPrice = cc.StartingFromPrice
                })
                .FirstOrDefaultAsync();

            return new ServiceResponse<CategoryConfigurationDto>
            {
                Data = configuration,
                Success = true,
                Message = "Configuration loaded successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configuration.");
            return new ServiceResponse<CategoryConfigurationDto>
            {
                Success = false,
                Message = "Error loading configuration."
            };
        }
    }

    public async Task<ServiceResponse<bool>> SaveCategoryConfigurationAsync(CategoryConfiguration configuration, byte[] bannerImage, string fileName)
    {
        try
        {
            _context.CategoryConfigurations.Add(configuration);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Success = true, Message = "Configuration saved successfully" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving configuration.");
            return new ServiceResponse<bool> { Success = false, Message = "Error saving configuration" };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteCategoryConfigurationAsync(long configurationId)
    {
        try
        {
            var configuration = await _context.CategoryConfigurations.FindAsync(configurationId);
            if (configuration == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Configuration not found" };
            }

            _context.CategoryConfigurations.Remove(configuration);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Success = true, Message = "Configuration deleted successfully" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting configuration.");
            return new ServiceResponse<bool> { Success = false, Message = "Error deleting configuration" };
        }
    }
    public async Task<ServiceResponse<CategoryConfigurationDto>> GetLastInsertedConfigurationAsync() // New method
    {
        try
        {
            var lastConfiguration = await _context.CategoryConfigurations
                .OrderByDescending(cc => cc.Id) // Assuming Id is auto-incrementing
                .Select(cc => new CategoryConfigurationDto
                {
                    Id = cc.Id,
                    CategoryId = cc.CategoryId,
                    CategoryName = cc.Category.Name,
                    BannerUrl = cc.BannerUrl,
                    Title = cc.Title,
                    Description = cc.Description,
                    StartingFromPrice = cc.StartingFromPrice
                })
                .FirstOrDefaultAsync();

            if (lastConfiguration == null)
            {
                return new ServiceResponse<CategoryConfigurationDto>
                {
                    Data = null,
                    Success = false,
                    Message = "No configurations found"
                };
            }

            return new ServiceResponse<CategoryConfigurationDto>
            {
                Data = lastConfiguration,
                Success = true,
                Message = "Last configuration loaded successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading last configuration.");
            return new ServiceResponse<CategoryConfigurationDto>
            {
                Success = false,
                Message = "Error loading last configuration"
            };
        }
    }

}
