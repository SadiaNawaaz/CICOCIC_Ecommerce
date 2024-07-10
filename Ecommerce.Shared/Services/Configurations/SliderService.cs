using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Configurations;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.Configurations;

public interface ISliderService
{
    Task<ServiceResponse<List<Slider>>> GetSlidersAsync();
    Task<ServiceResponse<Slider>> GetSliderByIdAsync(long id);
    Task<ServiceResponse<Slider>> AddSliderAsync(Slider slider);
    Task<ServiceResponse<Slider>> UpdateSliderAsync(Slider slider);
    Task<ServiceResponse<bool>> DeleteSliderAsync(long id);
}
public class SliderService : ISliderService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SliderService> _logger;

    public SliderService(ApplicationDbContext context, ILogger<SliderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Slider>>> GetSlidersAsync()
    {
        try
        {
            var sliders = await _context.Sliders.Include(s => s.Product).ToListAsync();
            return new ServiceResponse<List<Slider>>
            {
                Data = sliders,
                Success = true,
                Message = "Sliders fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching sliders.");
            return new ServiceResponse<List<Slider>>
            {
                Success = false,
                Message = "Error occurred while fetching sliders"
            };
        }
    }

    public async Task<ServiceResponse<Slider>> GetSliderByIdAsync(long id)
    {
        try
        {
            var slider = await _context.Sliders.Include(s => s.Product).FirstOrDefaultAsync(s => s.Id == id);
            return new ServiceResponse<Slider>
            {
                Data = slider,
                Success = true,
                Message = "Slider fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching slider by ID.");
            return new ServiceResponse<Slider>
            {
                Success = false,
                Message = "Error occurred while fetching slider by ID"
            };
        }
    }

    public async Task<ServiceResponse<Slider>> AddSliderAsync(Slider slider)
    {
        try
        {
            _context.Sliders.Add(slider);
            await _context.SaveChangesAsync();
            return new ServiceResponse<Slider>
            {
                Data = slider,
                Success = true,
                Message = "Slider added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding slider.");
            return new ServiceResponse<Slider>
            {
                Success = false,
                Message = "Error occurred while adding slider"
            };
        }
    }

    public async Task<ServiceResponse<Slider>> UpdateSliderAsync(Slider updatedSlider)
    {
        try
        {
            var existingSlider = await _context.Sliders.FindAsync(updatedSlider.Id);

            if (existingSlider == null)
            {
                return new ServiceResponse<Slider>
                {
                    Success = false,
                    Message = "Slider not found"
                };
            }

      
            existingSlider.BackgroundImageUrl = updatedSlider.BackgroundImageUrl;
            existingSlider.FrontImageUrl = updatedSlider.FrontImageUrl;
            existingSlider.Text = updatedSlider.Text;
            existingSlider.OrderNo = updatedSlider.OrderNo;
            existingSlider.Active = updatedSlider.Active;
            existingSlider.StartingFrom = updatedSlider.StartingFrom;
            existingSlider.ProductId = updatedSlider.ProductId;

            _context.Sliders.Update(existingSlider);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Slider>
            {
                Data = existingSlider,
                Success = true,
                Message = "Slider updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating slider.");
            return new ServiceResponse<Slider>
            {
                Success = false,
                Message = "Error occurred while updating slider"
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteSliderAsync(long id)
    {
        try
        {
            var slider = await _context.Sliders.FindAsync(id);

            if (slider == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Slider not found"
                };
            }

            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Slider deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting slider.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting slider"
            };
        }
    }
}