using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.CategoryFeatures;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.CategoryFeatures;

public interface ICategoryFeatureService
    {
    Task<ServiceResponse<List<CategoryFeature>>> GetCategoryFeaturesAsync();
    Task<ServiceResponse<CategoryFeature>> GetCategoryFeatureByIdAsync(int id);
    Task<ServiceResponse<CategoryFeature>> AddCategoryFeatureAsync(CategoryFeature categoryFeature);
    Task<ServiceResponse<CategoryFeature>> UpdateCategoryFeatureAsync(CategoryFeature updatedCategoryFeature);
    Task<ServiceResponse<bool>> DeleteCategoryFeatureAsync(int id);
    Task<ServiceResponse<bool>> ImportCategoryFeaturesAsync(List<CategoryFeature> categoryFeatures);
    }


public class CategoryFeatureService : ICategoryFeatureService
    {
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<CategoryFeatureService> _logger;

    public CategoryFeatureService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<CategoryFeatureService> logger)
        {
        _contextFactory = contextFactory;
        _logger = logger;
        }

    public async Task<ServiceResponse<List<CategoryFeature>>> GetCategoryFeaturesAsync()
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            var categoryFeatures = await context.CategoryFeatures.ToListAsync();
            return new ServiceResponse<List<CategoryFeature>>
                {
                Data = categoryFeatures,
                Success = true,
                Message = "Category features fetched successfully."
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching category features.");
            return new ServiceResponse<List<CategoryFeature>>
                {
                Success = false,
                Message = "Error occurred while fetching category features."
                };
            }
        }

    public async Task<ServiceResponse<CategoryFeature>> GetCategoryFeatureByIdAsync(int id)
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            var categoryFeature = await context.CategoryFeatures.FindAsync(id);
            if (categoryFeature == null)
                {
                return new ServiceResponse<CategoryFeature>
                    {
                    Success = false,
                    Message = "Category feature not found."
                    };
                }
            return new ServiceResponse<CategoryFeature>
                {
                Data = categoryFeature,
                Success = true,
                Message = "Category feature fetched successfully."
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching category feature by ID.");
            return new ServiceResponse<CategoryFeature>
                {
                Success = false,
                Message = "Error occurred while fetching category feature by ID."
                };
            }
        }

    public async Task<ServiceResponse<CategoryFeature>> AddCategoryFeatureAsync(CategoryFeature categoryFeature)
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            context.CategoryFeatures.Add(categoryFeature);
            await context.SaveChangesAsync();
            return new ServiceResponse<CategoryFeature>
                {
                Data = categoryFeature,
                Success = true,
                Message = "Category feature added successfully."
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while adding category feature.");
            return new ServiceResponse<CategoryFeature>
                {
                Success = false,
                Message = "Error occurred while adding category feature."
                };
            }
        }

    public async Task<ServiceResponse<CategoryFeature>> UpdateCategoryFeatureAsync(CategoryFeature updatedCategoryFeature)
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            var existingCategoryFeature = await context.CategoryFeatures.FindAsync(updatedCategoryFeature.Id);
            if (existingCategoryFeature == null)
                {
                return new ServiceResponse<CategoryFeature>
                    {
                    Success = false,
                    Message = "Category feature not found."
                    };
                }

            context.Entry(existingCategoryFeature).CurrentValues.SetValues(updatedCategoryFeature);
            await context.SaveChangesAsync();

            return new ServiceResponse<CategoryFeature>
                {
                Data = existingCategoryFeature,
                Success = true,
                Message = "Category feature updated successfully."
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while updating category feature.");
            return new ServiceResponse<CategoryFeature>
                {
                Success = false,
                Message = "Error occurred while updating category feature."
                };
            }
        }

    public async Task<ServiceResponse<bool>> DeleteCategoryFeatureAsync(int id)
        {
        try
            {
            using var context = _contextFactory.CreateDbContext();
            var categoryFeature = await context.CategoryFeatures.FindAsync(id);
            if (categoryFeature == null)
                {
                return new ServiceResponse<bool>
                    {
                    Success = false,
                    Message = "Category feature not found."
                    };
                }

            context.CategoryFeatures.Remove(categoryFeature);
            await context.SaveChangesAsync();

            return new ServiceResponse<bool>
                {
                Data = true,
                Success = true,
                Message = "Category feature deleted successfully."
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while deleting category feature.");
            return new ServiceResponse<bool>
                {
                Success = false,
                Message = "Error occurred while deleting category feature."
                };
            }
        }


    public async Task<ServiceResponse<bool>> ImportCategoryFeaturesAsync(List<CategoryFeature> categoryFeatures)
        {
        using var context = _contextFactory.CreateDbContext();
        using var transaction = context.Database.BeginTransaction();
        try
            {
            // Check if any provided category features have predefined IDs
            if (categoryFeatures.Any(cf => cf.Id != 0))
                {
                // Enable IDENTITY_INSERT for the CategoryFeatures table
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT CategoryFeatures ON");
                }

            context.CategoryFeatures.AddRange(categoryFeatures);
            await context.SaveChangesAsync();

            // Disable IDENTITY_INSERT for the CategoryFeatures table
            if (categoryFeatures.Any(cf => cf.Id != 0))
                {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT CategoryFeatures OFF");
                }

            transaction.Commit();
            return new ServiceResponse<bool>
                {
                Data = true,
                Success = true,
                Message = "Category features imported successfully."
                };
            }
        catch (Exception ex)
            {
            transaction.Rollback();
            _logger.LogError(ex, "Error occurred while importing category features.");
            return new ServiceResponse<bool>
                {
                Success = false,
                Message = "Error occurred while importing category features."
                };
            }
        }

    }

