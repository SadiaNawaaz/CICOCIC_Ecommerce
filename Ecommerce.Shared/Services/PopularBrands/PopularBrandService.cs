using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Entities.PopularBrands;
using Ecommerce.Shared.Services.Brands;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.PopularBrands;
public interface IPopularBrandService
{

    Task<ServiceResponse<List<PopularBrandDto>>> GetPopularBrandsAsync();
    Task<ServiceResponse<bool>> AddPopularBrandAsync(PopularBrand popularBrand, byte[] bannerImage, string fileName);
    Task<ServiceResponse<bool>> RemovePopularBrandAsync(long Id);
}

public class PopularBrandService : IPopularBrandService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<PopularBrandService> _logger;



    public PopularBrandService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<PopularBrandService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }
    public async Task<ServiceResponse<List<PopularBrandDto>>> GetPopularBrandsAsync()
    {
        try
        {
            using var _context = _contextFactory.CreateDbContext();
            var popularBrands = await _context.PopularBrands
                .Select(pb => new PopularBrandDto
                {
                    Id = pb.Id,
                    Name = pb.Brand.Name,
                    BannerUrl = pb.BannerUrl,
                    BrandId = pb.BrandId,
                    Order=pb.order
                })
                .ToListAsync();

            return new ServiceResponse<List<PopularBrandDto>>
            {
                Data = popularBrands,
                Success = true,
                Message = "Popular brands loaded successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading popular brands.");
            return new ServiceResponse<List<PopularBrandDto>>
            {
                Success = false,
                Message = "Error loading popular brands."
            };
        }
    }

    public async Task<ServiceResponse<bool>> AddPopularBrandAsync(PopularBrand popularBrand, byte[] bannerImage, string fileName)
    {
        try
        {
            using var _context = _contextFactory.CreateDbContext();
            var brand = await _context.Brands.FindAsync(popularBrand.BrandId);
            if (brand == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Brand not found" };
            }

    
            _context.PopularBrands.Add(popularBrand);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Success = true, Message = "Popular brand added successfully" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding popular brand.");
            return new ServiceResponse<bool> { Success = false, Message = "Error adding popular brand" };
        }
    }

    public async Task<ServiceResponse<bool>> RemovePopularBrandAsync(long Id)
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var popularBrand = await _context.PopularBrands.FirstOrDefaultAsync(pb => pb.Id == Id);
            if (popularBrand == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Popular brand not found" };
            }

            _context.PopularBrands.Remove(popularBrand);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Success = true, Message = "Popular brand removed successfully" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing popular brand.");
            return new ServiceResponse<bool> { Success = false, Message = "Error removing popular brand" };
        }
    }
}

