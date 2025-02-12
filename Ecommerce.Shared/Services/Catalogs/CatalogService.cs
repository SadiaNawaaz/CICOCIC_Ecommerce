﻿

using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Catalogs;
using Ecommerce.Shared.Migrations;
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
    Task<ServiceResponse<List<CatalogDto>>> SearchCatalogsAsync(string searchTerm);
    Task<ServiceResponse<List<CatalogDto>>> GetCatalogsWithBrandAsync();
    Task<ServiceResponse<List<Catalog>>> AddCatalogsAsync(List<Catalog> catalogs);
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


    public async Task<ServiceResponse<List<CatalogDto>>> GetCatalogsWithBrandAsync()
        {
        try
            {
            // Project only the needed columns into CatalogDto
            var catalogs = await _context.Catalogs
                .Include(c => c.Brand)
                .Select(c => new CatalogDto
                    {
                    Id = c.Id,
                    Name = (c.Brand != null ? c.Brand.Name : "No Brand") + " - " + c.Name,
                    BrandName = c.Brand != null ? c.Brand.Name : "No Brand",
                    Thumbnail = c.Thumbnail,
                    Price = c.Price,
                    Code = c.Code,
                    Mark=c.MarkProduct,
                    IntegratedId=c.IntegratedId,
                    EanNumber=c.EanNumber,
                    })
                .ToListAsync();

            return new ServiceResponse<List<CatalogDto>>
                {
                Data = catalogs,
                Success = true,
                Message = "Catalogs fetched successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching catalogs.");
            return new ServiceResponse<List<CatalogDto>>
                {
                Success = false,
                Message = "Error occurred while fetching catalogs"
                };
            }
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
                .Include(c => c.Brand)                  
                .Include(c => c.Category)               
                .Include(c => c.GeneralSize)            
                .Include(c => c.GeneralColor)           
                .Include(c => c.ModelYear)
                .Include(c => c.CatalogMedias)
                .Include(c => c.CatalogClusters)        
                    .ThenInclude(cc => cc.Cluster)      
                .Include(c => c.CatalogClusters)       
                    .ThenInclude(cc => cc.CatalogClusterFeatures)  
                        .ThenInclude(cf => cf.Feature)  
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


    public async Task<ServiceResponse<List<CatalogDto>>> SearchCatalogsAsync(string searchTerm)
        {
        try
            {
            var catalogs = await _context.Catalogs
                .Where(c => c.Name.Contains(searchTerm) ||
                            c.Description.Contains(searchTerm) ||
                            c.ShortDescription.Contains(searchTerm) ||
                            c.Code.Contains(searchTerm) ||
                            c.Brand.Name.Contains(searchTerm)||
                            (c.Brand != null && (c.Brand.Name + " " + c.Name).Contains(searchTerm)) ||
                            (c.Brand != null && (c.Brand.Name + " - " + c.Name).Contains(searchTerm))
                            )
                .ToListAsync();
            var catalogDtos = catalogs.Select(c => new CatalogDto
                {
                Id = c.Id,
                Name = (c.Brand != null ? $"{c.Brand.Name} " : "") + c.Name,
                BrandName = c.Brand.Name,  
                Thumbnail = c.Thumbnail,  
                Price = c.Price,          
                Code = c.Code
                }).ToList();

            return new ServiceResponse<List<CatalogDto>>
                {
                Data = catalogDtos,
                Success = true,
                Message = "Catalogs fetched successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while searching catalogs.");
            return new ServiceResponse<List<CatalogDto>>
                {
                Success = false,
                Message = "Error occurred while searching catalogs"
                };
            }
        }

    //    public async Task<ServiceResponse<List<Catalog>>> SearchCatalogsAsync(string searchTerm)
    //{
    //    try
    //    {
    //        var catalogs = await _context.Catalogs
    //            .Where(c => c.Name.Contains(searchTerm) || 
    //                        c.Description.Contains(searchTerm) || 
    //                        c.ShortDescription.Contains(searchTerm)|| c.Code.Contains(searchTerm)|| c.Brand.Name.Contains(searchTerm))
    //            .ToListAsync();

    //        return new ServiceResponse<List<Catalog>>
    //        {
    //            Data = catalogs,
    //            Success = true,
    //            Message = "Catalogs fetched successfully"
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error occurred while searching catalogs.");
    //        return new ServiceResponse<List<Catalog>>
    //        {
    //            Success = false,
    //            Message = "Error occurred while searching catalogs"
    //        };
    //    }
    //}



    public async Task<ServiceResponse<List<Catalog>>> AddCatalogsAsync(List<Catalog> catalogs)
        {
        try
            {
            _context.Catalogs.AddRange(catalogs);
            await _context.SaveChangesAsync();
            return new ServiceResponse<List<Catalog>>
                {
                Data = catalogs,
                Success = true,
                Message = "Catalogs added successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while adding catalogs.");
            return new ServiceResponse<List<Catalog>>
                {
                Success = false,
                Message = "Error occurred while adding catalogs"
                };
            }
        }

    }
