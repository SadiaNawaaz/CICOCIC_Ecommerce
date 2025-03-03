

using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.Brands;

public interface IBrandService
{
    Task<ServiceResponse<List<Brand>>> GetBrandsAsync();
    Task<ServiceResponse<List<Brand>>> GetTopBrandsAsync();
    Task<ServiceResponse<Brand>> GetBrandByIdAsync(long id);
    Task<ServiceResponse<Brand>> AddBrandAsync(Brand brand);
    Task<ServiceResponse<Brand>> UpdateBrandAsync(Brand brand);
    Task<ServiceResponse<bool>> DeleteBrandAsync(long id);
    Task<ServiceResponse<bool>> ImportBrandsAsync(List<Brand> brands);
    Task<ServiceResponse<List<BrandDto>>> GetBrandsDtoAsync();
    Task<ServiceResponse<Brand>> MarkUnmarkBrandAsync(long id, bool isMarked);
    Task<ServiceResponse<List<BrandDto>>> GetAllBrandsDtoAsync();
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


    public async Task<ServiceResponse<List<Brand>>> GetTopBrandsAsync()
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            var brands = await context.Brands.Take(10).ToListAsync();
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


    public async Task<ServiceResponse<bool>> ImportBrandsAsync(List<Brand> brands)
        {
        using var context = _contextFactory.CreateDbContext();
        using var transaction = context.Database.BeginTransaction();
        try
            {
            // Check if there are any brands to insert with specified IDs
            if (brands.Any(b => b.Id != 0))
                {
                // Enable IDENTITY_INSERT before adding new brands
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Brands ON");
                }

            var existingBrandIds = brands.Select(b => b.Id).ToList();
            var existingBrands = await context.Brands
                .Where(b => existingBrandIds.Contains(b.Id))
                .ToListAsync();

            var newBrands = brands.Where(b => !existingBrands.Any(eb => eb.Id == b.Id)).ToList();
            var updatedBrands = brands.Where(b => existingBrands.Any(eb => eb.Id == b.Id)).ToList();

            if (newBrands.Any())
                context.Brands.AddRange(newBrands);

            foreach (var updatedBrand in updatedBrands)
                {
                var existingBrand = existingBrands.FirstOrDefault(eb => eb.Id == updatedBrand.Id);
                if (existingBrand != null)
                    {
                    context.Entry(existingBrand).CurrentValues.SetValues(updatedBrand);
                    }
                }

            await context.SaveChangesAsync();

            // Disable IDENTITY_INSERT after operations
            if (brands.Any(b => b.Id != 0))
                {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Brands OFF");
                }

            transaction.Commit();

            return new ServiceResponse<bool>
                {
                Data = true,
                Success = true,
                Message = "Brands imported successfully"
                };
            }
        catch (Exception ex)
            {
            transaction.Rollback();
            _logger.LogError(ex, "Error occurred while importing brands.");
            return new ServiceResponse<bool>
                {
                Success = false,
                Message = "Error occurred while importing brands"
                };
            }
        }

    public async Task<ServiceResponse<List<BrandDto>>> GetBrandsDtoAsync()
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            var brands = await context.Brands.Where(a=>a.MarkBrand==true)
                
                .Select(b => new BrandDto
                    {
                    Id = b.Id,
                    Name = b.Name
                    })
                .ToListAsync();

            return new ServiceResponse<List<BrandDto>>
                {
                Data = brands,
                Success = true,
                Message = "Brands fetched successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching brands.");
            return new ServiceResponse<List<BrandDto>>
                {
                Success = false,
                Message = "Error occurred while fetching brands"
                };
            }
        }

    public async Task<ServiceResponse<List<BrandDto>>> GetAllBrandsDtoAsync()
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            var brands = await context.Brands

                .Select(b => new BrandDto
                    {
                    Id = b.Id,
                    Name = b.Name,
                    MarkBrand=b.MarkBrand
                    })
                .ToListAsync();

            return new ServiceResponse<List<BrandDto>>
                {
                Data = brands,
                Success = true,
                Message = "Brands fetched successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching brands.");
            return new ServiceResponse<List<BrandDto>>
                {
                Success = false,
                Message = "Error occurred while fetching brands"
                };
            }
        }

    public async Task<ServiceResponse<Brand>> MarkUnmarkBrandAsync(long id, bool isMarked)
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

            brand.MarkBrand = isMarked;
            context.Brands.Update(brand);
            await context.SaveChangesAsync();

            return new ServiceResponse<Brand>
                {
                Data = brand,
                Success = true,
                Message = isMarked ? "Brand marked successfully" : "Brand unmarked successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while marking/unmarking brand.");
            return new ServiceResponse<Brand>
                {
                Success = false,
                Message = "Error occurred while marking/unmarking brand"
                };
            }
        }

    }
