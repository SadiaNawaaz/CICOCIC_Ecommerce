

using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Features;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.Features;

public interface IFeatureService
{
    Task<ServiceResponse<List<Feature>>> GetFeaturesAsync();
    Task<ServiceResponse<Feature>> GetFeatureByIdAsync(long id);
    Task<ServiceResponse<Feature>> AddFeatureAsync(Feature feature);
    Task<ServiceResponse<Feature>> UpdateFeatureAsync(Feature feature);
    Task<ServiceResponse<bool>> DeleteFeatureAsync(long id);
    Task<ServiceResponse<List<Feature>>> GetFeaturesByClusterId(long clusterId);
    Task<ServiceResponse<bool>> ImportFeaturesAsync(List<Feature> features);
}
public class FeatureService : IFeatureService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<FeatureService> _logger;

    public FeatureService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<FeatureService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Feature>>> GetFeaturesAsync()
    {
        try
        {
            using var _context = _contextFactory.CreateDbContext();
            var features = await _context.Features.ToListAsync();
            return new ServiceResponse<List<Feature>>
            {
                Data = features,
                Success = true,
                Message = "Features fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching features.");
            return new ServiceResponse<List<Feature>>
            {
                Success = false,
                Message = "Error occurred while fetching features"
            };
        }
    }

    public async Task<ServiceResponse<Feature>> GetFeatureByIdAsync(long id)
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var feature = await _context.Features.FindAsync(id);
            return new ServiceResponse<Feature>
            {
                Data = feature,
                Success = true,
                Message = "Feature fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching feature by ID.");
            return new ServiceResponse<Feature>
            {
                Success = false,
                Message = "Error occurred while fetching feature by ID"
            };
        }
    }

    public async Task<ServiceResponse<Feature>> AddFeatureAsync(Feature feature)
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            _context.Features.Add(feature);
            await _context.SaveChangesAsync();
            return new ServiceResponse<Feature>
            {
                Data = feature,
                Success = true,
                Message = "Feature added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding feature.");
            return new ServiceResponse<Feature>
            {
                Success = false,
                Message = "Error occurred while adding feature"
            };
        }
    }

    public async Task<ServiceResponse<Feature>> UpdateFeatureAsync(Feature updatedFeature)
    {
        try
        {
            using var _context = _contextFactory.CreateDbContext();
            var existingFeature = await _context.Features.FindAsync(updatedFeature.Id);

            if (existingFeature == null)
            {
                return new ServiceResponse<Feature>
                {
                    Success = false,
                    Message = "Feature not found"
                };
            }

            existingFeature.Name = updatedFeature.Name;
            _context.Features.Update(existingFeature);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Feature>
            {
                Data = existingFeature,
                Success = true,
                Message = "Feature updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating feature.");
            return new ServiceResponse<Feature>
            {
                Success = false,
                Message = "Error occurred while updating feature"
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteFeatureAsync(long id)
    {
        try
        {
            using var _context = _contextFactory.CreateDbContext();
            var feature = await _context.Features.FindAsync(id);

            if (feature == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Feature not found"
                };
            }

            _context.Features.Remove(feature);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Feature deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting feature.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting feature"
            };
        }
    }


    public async Task<ServiceResponse<List<Feature>>> GetFeaturesByClusterId(long clusterId)
    {
        try
        {
            using var _context = _contextFactory.CreateDbContext();
            var features = await _context.Features.Where(a=>a.ClusterId== clusterId).ToListAsync();
            return new ServiceResponse<List<Feature>>
            {
                Data = features,
                Success = true,
                Message = "Features fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching features.");
            return new ServiceResponse<List<Feature>>
            {
                Success = false,
                Message = "Error occurred while fetching features"
            };
        }
    }
    public async Task<ServiceResponse<bool>> ImportFeaturesAsync(List<Feature> features)
        {
        using var context = _contextFactory.CreateDbContext();
        using var transaction = context.Database.BeginTransaction();

        try
            {
            // Check if there are any features to insert with specified IDs
            if (features.Any(f => f.Id != 0))
                {
                // Enable IDENTITY_INSERT before adding new features
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Features ON");
                }

            var existingFeatureIds = features.Select(f => f.Id).ToList();
            var existingFeatures = await context.Features
                .Where(f => existingFeatureIds.Contains(f.Id))
                .ToListAsync();

            var newFeatures = features.Where(f => !existingFeatures.Any(ef => ef.Id == f.Id)).ToList();
            var updatedFeatures = features.Where(f => existingFeatures.Any(ef => ef.Id == f.Id)).ToList();

            if (newFeatures.Any())
                {
                context.Features.AddRange(newFeatures);
                }

            foreach (var updatedFeature in updatedFeatures)
                {
                var existingFeature = existingFeatures.FirstOrDefault(ef => ef.Id == updatedFeature.Id);
                if (existingFeature != null)
                    {
                    context.Entry(existingFeature).CurrentValues.SetValues(updatedFeature);
                    }
                }

            await context.SaveChangesAsync();

            // Disable IDENTITY_INSERT after operations
            if (features.Any(f => f.Id != 0))
                {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Features OFF");
                }

            transaction.Commit();

            return new ServiceResponse<bool>
                {
                Data = true,
                Success = true,
                Message = "Features imported successfully"
                };
            }
        catch (Exception ex)
            {
            transaction.Rollback();
            _logger.LogError(ex, "Error occurred while importing features.");
            return new ServiceResponse<bool>
                {
                Success = false,
                Message = "Error occurred while importing features"
                };
            }
        }



    }
