﻿

using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.Brands;

public interface IBrandService
{
    Task<ServiceResponse<List<Brand>>> GetBrandsAsync();
    Task<ServiceResponse<Brand>> GetBrandByIdAsync(long id);
    Task<ServiceResponse<Brand>> AddBrandAsync(Brand brand);
    Task<ServiceResponse<Brand>> UpdateBrandAsync(Brand brand);
    Task<ServiceResponse<bool>> DeleteBrandAsync(long id);
}
public class BrandService : IBrandService
    {
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<BrandService> _logger;

    public BrandService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<BrandService> logger)
        {
        _contextFactory = contextFactory;
        _logger = logger;
        }

    public async Task<ServiceResponse<List<Brand>>> GetBrandsAsync()
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            var brands = await context.Brands.ToListAsync();
            return new ServiceResponse<List<Brand>>
                {
                Data = brands,
                Success = true,
                Message = "Brands fetched successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching brands.");
            return new ServiceResponse<List<Brand>>
                {
                Success = false,
                Message = "Error occurred while fetching brands"
                };
            }
        }

    public async Task<ServiceResponse<Brand>> GetBrandByIdAsync(long id)
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            var brand = await context.Brands.FindAsync(id);
            if (brand == null)
                {
                return new ServiceResponse<Brand>
                    {
                    Success = false,
                    Message = "Brand not found"
                    };
                }
            return new ServiceResponse<Brand>
                {
                Data = brand,
                Success = true,
                Message = "Brand fetched successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching brand by ID.");
            return new ServiceResponse<Brand>
                {
                Success = false,
                Message = "Error occurred while fetching brand by ID"
                };
            }
        }

    public async Task<ServiceResponse<Brand>> AddBrandAsync(Brand brand)
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            context.Brands.Add(brand);
            await context.SaveChangesAsync();
            return new ServiceResponse<Brand>
                {
                Data = brand,
                Success = true,
                Message = "Brand added successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while adding brand.");
            return new ServiceResponse<Brand>
                {
                Success = false,
                Message = "Error occurred while adding brand"
                };
            }
        }

    public async Task<ServiceResponse<Brand>> UpdateBrandAsync(Brand updatedBrand)
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            var existingBrand = await context.Brands.FindAsync(updatedBrand.Id);
            if (existingBrand == null)
                {
                return new ServiceResponse<Brand>
                    {
                    Success = false,
                    Message = "Brand not found"
                    };
                }

            existingBrand.Name = updatedBrand.Name;
            existingBrand.ImageUrl = updatedBrand.ImageUrl;
            context.Brands.Update(existingBrand);
            await context.SaveChangesAsync();

            return new ServiceResponse<Brand>
                {
                Data = existingBrand,
                Success = true,
                Message = "Brand updated successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while updating brand.");
            return new ServiceResponse<Brand>
                {
                Success = false,
                Message = "Error occurred while updating brand"
                };
            }
        }

    public async Task<ServiceResponse<bool>> DeleteBrandAsync(long id)
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            var brand = await context.Brands.FindAsync(id);
            if (brand == null)
                {
                return new ServiceResponse<bool>
                    {
                    Success = false,
                    Message = "Brand not found"
                    };
                }

            context.Brands.Remove(brand);
            await context.SaveChangesAsync();

            return new ServiceResponse<bool>
                {
                Data = true,
                Success = true,
                Message = "Brand deleted successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while deleting brand.");
            return new ServiceResponse<bool>
                {
                Success = false,
                Message = "Error occurred while deleting brand"
                };
            }
        }
    }
