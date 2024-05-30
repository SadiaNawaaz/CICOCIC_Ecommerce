

using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.ModelYears;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.ModelYears;

public interface IModelYearService
{
    Task<ServiceResponse<List<ModelYear>>> GetModelYearsAsync();
    Task<ServiceResponse<ModelYear>> GetModelYearByIdAsync(long id);
    Task<ServiceResponse<ModelYear>> AddModelYearAsync(ModelYear modelYear);
    Task<ServiceResponse<ModelYear>> UpdateModelYearAsync(ModelYear modelYear);
    Task<ServiceResponse<bool>> DeleteModelYearAsync(long id);
}
public class ModelYearService : IModelYearService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ModelYearService> _logger;

    public ModelYearService(ApplicationDbContext context, ILogger<ModelYearService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<ModelYear>>> GetModelYearsAsync()
    {
        try
        {
            var modelYears = await _context.ModelYears.ToListAsync();
            return new ServiceResponse<List<ModelYear>>
            {
                Data = modelYears,
                Success = true,
                Message = "Model years fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching model years.");
            return new ServiceResponse<List<ModelYear>>
            {
                Success = false,
                Message = "Error occurred while fetching model years"
            };
        }
    }

    public async Task<ServiceResponse<ModelYear>> GetModelYearByIdAsync(long id)
    {
        try
        {
            var modelYear = await _context.ModelYears.FindAsync(id);
            if (modelYear == null)
            {
                return new ServiceResponse<ModelYear>
                {
                    Success = false,
                    Message = "Model year not found"
                };
            }

            return new ServiceResponse<ModelYear>
            {
                Data = modelYear,
                Success = true,
                Message = "Model year fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching model year by ID.");
            return new ServiceResponse<ModelYear>
            {
                Success = false,
                Message = "Error occurred while fetching model year by ID"
            };
        }
    }

    public async Task<ServiceResponse<ModelYear>> AddModelYearAsync(ModelYear modelYear)
    {
        try
        {
            _context.ModelYears.Add(modelYear);
            await _context.SaveChangesAsync();
            return new ServiceResponse<ModelYear>
            {
                Data = modelYear,
                Success = true,
                Message = "Model year added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding model year.");
            return new ServiceResponse<ModelYear>
            {
                Success = false,
                Message = "Error occurred while adding model year"
            };
        }
    }

    public async Task<ServiceResponse<ModelYear>> UpdateModelYearAsync(ModelYear updatedModelYear)
    {
        try
        {
            var existingModelYear = await _context.ModelYears.FindAsync(updatedModelYear.Id);
            if (existingModelYear == null)
            {
                return new ServiceResponse<ModelYear>
                {
                    Success = false,
                    Message = "Model year not found"
                };
            }

            existingModelYear.Year = updatedModelYear.Year;
            _context.ModelYears.Update(existingModelYear);
            await _context.SaveChangesAsync();

            return new ServiceResponse<ModelYear>
            {
                Data = existingModelYear,
                Success = true,
                Message = "Model year updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating model year.");
            return new ServiceResponse<ModelYear>
            {
                Success = false,
                Message = "Error occurred while updating model year"
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteModelYearAsync(long id)
    {
        try
        {
            var modelYear = await _context.ModelYears.FindAsync(id);
            if (modelYear == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Model year not found"
                };
            }

            _context.ModelYears.Remove(modelYear);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Model year deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting model year.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting model year"
            };
        }
    }
}
