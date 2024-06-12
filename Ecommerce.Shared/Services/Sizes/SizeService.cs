

using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Sizes;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.Sizes;

public interface ISizeService
{
    Task<ServiceResponse<List<GeneralSize>>> GetSizesAsync();
    Task<ServiceResponse<GeneralSize>> GetSizeByIdAsync(long id);
    Task<ServiceResponse<GeneralSize>> AddSizeAsync(GeneralSize size);
    Task<ServiceResponse<GeneralSize>> UpdateSizeAsync(GeneralSize size);
    Task<ServiceResponse<bool>> DeleteSizeAsync(long id);
}
public class SizeService : ISizeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SizeService> _logger;

    public SizeService(ApplicationDbContext context, ILogger<SizeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<GeneralSize>>> GetSizesAsync()
    {
        try
        {
            var sizes = await _context.Sizes.ToListAsync();
            return new ServiceResponse<List<GeneralSize>>
            {
                Data = sizes,
                Success = true,
                Message = "Sizes fetched successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching sizes.");
            return new ServiceResponse<List<GeneralSize>>
            {
                Success = false,
                Message = "Error occurred while fetching sizes."
            };
        }
    }

    public async Task<ServiceResponse<GeneralSize>> GetSizeByIdAsync(long id)
    {
        try
        {
            var size = await _context.Sizes.FindAsync(id);
            if (size != null)
            {
                return new ServiceResponse<GeneralSize>
                {
                    Data = size,
                    Success = true,
                    Message = "Size fetched successfully."
                };
            }
            else
            {
                return new ServiceResponse<GeneralSize>
                {
                    Success = false,
                    Message = "Size not found."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching the size by ID.");
            return new ServiceResponse<GeneralSize>
            {
                Success = false,
                Message = "Error occurred while fetching the size by ID."
            };
        }
    }

    public async Task<ServiceResponse<GeneralSize>> AddSizeAsync(GeneralSize size)
    {
        try
        {
            _context.Sizes.Add(size);
            await _context.SaveChangesAsync();
            return new ServiceResponse<GeneralSize>
            {
                Data = size,
                Success = true,
                Message = "Size added successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding the size.");
            return new ServiceResponse<GeneralSize>
            {
                Success = false,
                Message = "Error occurred while adding the size."
            };
        }
    }

    public async Task<ServiceResponse<GeneralSize>> UpdateSizeAsync(GeneralSize updatedSize)
    {
        try
        {
            var existingSize = await _context.Sizes.FindAsync(updatedSize.Id);
            if (existingSize == null)
            {
                return new ServiceResponse<GeneralSize>
                {
                    Success = false,
                    Message = "Size not found."
                };
            }

            existingSize.Name = updatedSize.Name;
            _context.Sizes.Update(existingSize);
            await _context.SaveChangesAsync();

            return new ServiceResponse<GeneralSize>
            {
                Data = existingSize,
                Success = true,
                Message = "Size updated successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating the size.");
            return new ServiceResponse<GeneralSize>
            {
                Success = false,
                Message = "Error occurred while updating the size."
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteSizeAsync(long id)
    {
        try
        {
            var size = await _context.Sizes.FindAsync(id);
            if (size == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Size not found."
                };
            }

            _context.Sizes.Remove(size);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Size deleted successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting the size.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting the size."
            };
        }
    }
}
