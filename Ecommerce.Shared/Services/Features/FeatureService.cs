

using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Features;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.Features;

public interface IFeatureService
{
    Task<ServiceResponse<List<Feature>>> GetFeaturesAsync();
    Task<ServiceResponse<Feature>> GetFeatureByIdAsync(long id);
    Task<ServiceResponse<Feature>> AddFeatureAsync(Feature feature);
    Task<ServiceResponse<Feature>> UpdateFeatureAsync(Feature feature);
    Task<ServiceResponse<bool>> DeleteFeatureAsync(long id);
}
public class FeatureService : IFeatureService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FeatureService> _logger;

    public FeatureService(ApplicationDbContext context, ILogger<FeatureService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Feature>>> GetFeaturesAsync()
    {
        try
        {
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
}
