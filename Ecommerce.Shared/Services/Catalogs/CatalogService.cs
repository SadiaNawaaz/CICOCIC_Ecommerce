

using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Catalogs;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.Catalogs;

public interface ICatalogService
{
    Task<ServiceResponse<List<Catalog>>> GetCatalogsAsync();
    Task<ServiceResponse<Catalog>> GetCatalogByIdAsync(long id);
    Task<ServiceResponse<Catalog>> AddCatalogAsync(Catalog catalog);
    Task<ServiceResponse<Catalog>> UpdateCatalogAsync(Catalog catalog);
    Task<ServiceResponse<bool>> DeleteCatalogAsync(long id);
}
public class CatalogService: ICatalogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CatalogService> _logger;

    public CatalogService(ApplicationDbContext context, ILogger<CatalogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Catalog>>> GetCatalogsAsync()
    {
        try
        {
            var catalogs = await _context.Catalogs.ToListAsync();
            return new ServiceResponse<List<Catalog>>
            {
                Data = catalogs,
                Success = true,
                Message = "Catalogs fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching catalogs.");
            return new ServiceResponse<List<Catalog>>
            {
                Success = false,
                Message = "Error occurred while fetching catalogs"
            };
        }
    }

    public async Task<ServiceResponse<Catalog>> GetCatalogByIdAsync(long id)
    {
        try
        {
            // Use Include to load related entities
            var catalog = await _context.Catalogs
                .Include(c => c.Brand)                   // Include related Brand
                .Include(c => c.Category)                // Include related Category
                .Include(c => c.GeneralSize)             // Include related GeneralSize
                .Include(c => c.GeneralColor)            // Include related GeneralColor
                .Include(c => c.ModelYear)               // Include related ModelYear
                .Include(c => c.CatalogClusters)         // Include related CatalogClusters
                    .ThenInclude(cc => cc.Cluster)       // Include the Cluster within CatalogClusters
                .Include(c => c.CatalogClusters)         // Include related CatalogClusters again to include features
                    .ThenInclude(cc => cc.CatalogClusterFeatures)  // Include CatalogClusterFeatures within each Cluster
                        .ThenInclude(cf => cf.Feature)   // Include the Feature within each CatalogClusterFeature
                .FirstOrDefaultAsync(c => c.Id == id);
            return new ServiceResponse<Catalog>
            {
                Data = catalog,
                Success = true,
                Message = "Catalog fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching catalog by ID.");
            return new ServiceResponse<Catalog>
            {
                Success = false,
                Message = "Error occurred while fetching catalog by ID"
            };
        }
    }

    public async Task<ServiceResponse<Catalog>> AddCatalogAsync(Catalog catalog)
    {
        try
        {
            _context.Catalogs.Add(catalog);
            await _context.SaveChangesAsync();
            return new ServiceResponse<Catalog>
            {
                Data = catalog,
                Success = true,
                Message = "Catalog added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding catalog.");
            return new ServiceResponse<Catalog>
            {
                Success = false,
                Message = "Error occurred while adding catalog"
            };
        }
    }

    public async Task<ServiceResponse<Catalog>> UpdateCatalogAsync(Catalog updatedCatalog)
    {
        try
        {
            var existingCatalog = await _context.Catalogs.FindAsync(updatedCatalog.Id);

            if (existingCatalog == null)
            {
                return new ServiceResponse<Catalog>
                {
                    Success = false,
                    Message = "Catalog not found"
                };
            }

            existingCatalog.Name = updatedCatalog.Name;
            existingCatalog.Price = updatedCatalog.Price;
            existingCatalog.BrandId = updatedCatalog.BrandId;
            existingCatalog.GeneralSizeId = updatedCatalog.GeneralSizeId;
            existingCatalog.GeneralColorId = updatedCatalog.GeneralColorId;
            existingCatalog.ModelYearId = updatedCatalog.ModelYearId;
            existingCatalog.Thumbnail = updatedCatalog.Thumbnail;
            existingCatalog.CategoryId = updatedCatalog.CategoryId;
            // Update other properties as needed...

            _context.Catalogs.Update(existingCatalog);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Catalog>
            {
                Data = existingCatalog,
                Success = true,
                Message = "Catalog updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating catalog.");
            return new ServiceResponse<Catalog>
            {
                Success = false,
                Message = "Error occurred while updating catalog"
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteCatalogAsync(long id)
    {
        try
        {
            var catalog = await _context.Catalogs.FindAsync(id);

            if (catalog == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Catalog not found"
                };
            }

            _context.Catalogs.Remove(catalog);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Catalog deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting catalog.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting catalog"
            };
        }
    }

}
