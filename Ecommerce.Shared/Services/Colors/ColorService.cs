
using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Colors;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.Colors;

public interface IColorService
{
    Task<ServiceResponse<List<GeneralColor>>> GetColorsAsync();
    Task<ServiceResponse<GeneralColor>> GetColorByIdAsync(long id);
    Task<ServiceResponse<GeneralColor>> AddColorAsync(GeneralColor color);
    Task<ServiceResponse<GeneralColor>> UpdateColorAsync(GeneralColor color);
    Task<ServiceResponse<bool>> DeleteColorAsync(long id);
}

public class ColorService : IColorService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ColorService> _logger;

    public ColorService(ApplicationDbContext context, ILogger<ColorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<GeneralColor>>> GetColorsAsync()
    {
        try
        {
            var colors = await _context.Colors.ToListAsync();
            return new ServiceResponse<List<GeneralColor>>
            {
                Data = colors,
                Success = true,
                Message = "Colors fetched successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching colors.");
            return new ServiceResponse<List<GeneralColor>>
            {
                Success = false,
                Message = "Error occurred while fetching colors."
            };
        }
    }

    public async Task<ServiceResponse<GeneralColor>> GetColorByIdAsync(long id)
    {
        try
        {
            var color = await _context.Colors.FindAsync(id);
            if (color != null)
            {
                return new ServiceResponse<GeneralColor>
                {
                    Data = color,
                    Success = true,
                    Message = "Color fetched successfully."
                };
            }
            else
            {
                return new ServiceResponse<GeneralColor>
                {
                    Success = false,
                    Message = "Color not found."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching the color by ID.");
            return new ServiceResponse<GeneralColor>
            {
                Success = false,
                Message = "Error occurred while fetching the color by ID."
            };
        }
    }

    public async Task<ServiceResponse<GeneralColor>> AddColorAsync(GeneralColor color)
    {
        try
        {
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return new ServiceResponse<GeneralColor>
            {
                Data = color,
                Success = true,
                Message = "Color added successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding the color.");
            return new ServiceResponse<GeneralColor>
            {
                Success = false,
                Message = "Error occurred while adding the color."
            };
        }
    }

    public async Task<ServiceResponse<GeneralColor>> UpdateColorAsync(GeneralColor updatedColor)
    {
        try
        {
            var existingColor = await _context.Colors.FindAsync(updatedColor.Id);
            if (existingColor == null)
            {
                return new ServiceResponse<GeneralColor>
                {
                    Success = false,
                    Message = "Color not found."
                };
            }

            existingColor.Name = updatedColor.Name;
            existingColor.HexCode = updatedColor.HexCode;
            _context.Colors.Update(existingColor);
            await _context.SaveChangesAsync();

            return new ServiceResponse<GeneralColor>
            {
                Data = existingColor,
                Success = true,
                Message = "Color updated successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating the color.");
            return new ServiceResponse<GeneralColor>
            {
                Success = false,
                Message = "Error occurred while updating the color."
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteColorAsync(long id)
    {
        try
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Color not found."
                };
            }

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Color deleted successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting the color.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting the color."
            };
        }
    }
}