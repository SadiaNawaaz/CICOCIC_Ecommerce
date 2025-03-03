using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.PopularCategories;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.PopularCategories;

public interface IPopularCategoryService
{
    Task<ServiceResponse<List<PopularCategoryDto>>> GetPopularCategoriesAsync(string languageCode = "en");
    Task<ServiceResponse<bool>> AddPopularCategoryAsync(PopularCategory popularCategory, byte[] bannerImage, string fileName);
    Task<ServiceResponse<bool>> RemovePopularCategoryAsync(long Id);
}
public class PopularCategoryService : IPopularCategoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<PopularCategoryService> _logger;

    public PopularCategoryService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<PopularCategoryService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }


    public async Task<ServiceResponse<List<PopularCategoryDto>>> GetPopularCategoriesAsync(string languageCode = "en")
        {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var languageId = await _context.Languages
                .Where(l => l.LanguageCode == languageCode)
                .Select(l => l.Id)
                .FirstOrDefaultAsync();

            if (languageId == 0) 
                {
                languageId = await _context.Languages
                    .Where(l => l.LanguageCode == "en")
                    .Select(l => l.Id)
                    .FirstOrDefaultAsync();
                }

            var popularCategories = await _context.PopularCategories
                .Include(pc => pc.Category)
                .ThenInclude(c => c.Translations)
                .Select(pc => new PopularCategoryDto
                    {
                    Id = pc.Id,
                    CategoryId = pc.CategoryId,
                    BannerUrl = pc.BannerUrl,
                    Order = pc.Order,
                    Name = pc.Category.Translations
                        .Where(t => t.LanguageId == languageId)
                        .Select(t => t.TranslatedName)
                        .FirstOrDefault() ?? pc.Category.Name
                    })
                .ToListAsync();

            return new ServiceResponse<List<PopularCategoryDto>>
                {
                Data = popularCategories,
                Success = true,
                Message = "Popular categories loaded successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error loading popular categories.");
            return new ServiceResponse<List<PopularCategoryDto>>
                {
                Success = false,
                Message = "Error loading popular categories."
                };
            }
        }


    public async Task<ServiceResponse<bool>> AddPopularCategoryAsync(PopularCategory popularCategory, byte[] bannerImage, string fileName)
    {
        try
        {
            using var _context = _contextFactory.CreateDbContext();
            _context.PopularCategories.Add(popularCategory);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Success = true, Message = "Popular category added successfully" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding popular category.");
            return new ServiceResponse<bool> { Success = false, Message = "Error adding popular category" };
        }
    }

    public async Task<ServiceResponse<bool>> RemovePopularCategoryAsync(long Id)
    {
        try
        {
            using var _context = _contextFactory.CreateDbContext();
            var popularCategory = await _context.PopularCategories.FirstOrDefaultAsync(pc => pc.Id == Id);
            if (popularCategory == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Popular category not found" };
            }

            _context.PopularCategories.Remove(popularCategory);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Success = true, Message = "Popular category removed successfully" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing popular category.");
            return new ServiceResponse<bool> { Success = false, Message = "Error removing popular category" };
        }
    }
}

