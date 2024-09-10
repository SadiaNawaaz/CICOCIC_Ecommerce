using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.PopularCategories;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.PopularCategories;

public interface IPopularCategoryService
{
    Task<ServiceResponse<List<PopularCategoryDto>>> GetPopularCategoriesAsync();
    Task<ServiceResponse<bool>> AddPopularCategoryAsync(PopularCategory popularCategory, byte[] bannerImage, string fileName);
    Task<ServiceResponse<bool>> RemovePopularCategoryAsync(long Id);
}
public class PopularCategoryService : IPopularCategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PopularCategoryService> _logger;

    public PopularCategoryService(ApplicationDbContext context, ILogger<PopularCategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<PopularCategoryDto>>> GetPopularCategoriesAsync()
    {
        try
        {
            var popularCategories = await _context.PopularCategories
                .Select(pc => new PopularCategoryDto
                {
                    Id = pc.Id,
                    Name = pc.Category.Name,
                    BannerUrl = pc.BannerUrl,
                    CategoryId = pc.CategoryId,
                    Order = pc.Order
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

