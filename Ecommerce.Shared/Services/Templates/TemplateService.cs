

using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Templates;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.Templates;

public interface ITemplateService
{
    Task<ServiceResponse<List<TemplateMaster>>> GetTemplateMastersAsync();
    Task<ServiceResponse<TemplateMaster>> GetTemplateMasterByIdAsync(long id);
    Task<ServiceResponse<TemplateMaster>> AddTemplateMasterAsync(TemplateMaster templateMaster);
    Task<ServiceResponse<TemplateMaster>> UpdateTemplateMasterAsync(TemplateMaster templateMaster);
    Task<ServiceResponse<bool>> DeleteTemplateMasterAsync(long id);
}
public class TemplateService : ITemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TemplateService> _logger;

    public TemplateService(ApplicationDbContext context, ILogger<TemplateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<TemplateMaster>>> GetTemplateMastersAsync()
    {
        try
        {
            var templateMasters = await _context.TemplateMasters.ToListAsync();
            return new ServiceResponse<List<TemplateMaster>>
            {
                Data = templateMasters,
                Success = true,
                Message = "Template masters fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching template masters.");
            return new ServiceResponse<List<TemplateMaster>>
            {
                Success = false,
                Message = "Error occurred while fetching template masters"
            };
        }
    }

    public async Task<ServiceResponse<TemplateMaster>> GetTemplateMasterByIdAsync(long id)
    {
        try
        {
            var templateMaster = await _context.TemplateMasters.FindAsync(id);
            if (templateMaster == null)
            {
                return new ServiceResponse<TemplateMaster>
                {
                    Success = false,
                    Message = "Template master not found"
                };
            }

            return new ServiceResponse<TemplateMaster>
            {
                Data = templateMaster,
                Success = true,
                Message = "Template master fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while fetching template master by ID: {id}");
            return new ServiceResponse<TemplateMaster>
            {
                Success = false,
                Message = $"Error occurred while fetching template master by ID: {id}"
            };
        }
    }

    public async Task<ServiceResponse<TemplateMaster>> AddTemplateMasterAsync(TemplateMaster templateMaster)
    {
        try
        {
            _context.TemplateMasters.Add(templateMaster);
            await _context.SaveChangesAsync();
            return new ServiceResponse<TemplateMaster>
            {
                Data = templateMaster,
                Success = true,
                Message = "Template master added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding template master.");
            return new ServiceResponse<TemplateMaster>
            {
                Success = false,
                Message = "Error occurred while adding template master"
            };
        }
    }

    public async Task<ServiceResponse<TemplateMaster>> UpdateTemplateMasterAsync(TemplateMaster updatedTemplateMaster)
    {
        try
        {
            var existingTemplateMaster = await _context.TemplateMasters.FindAsync(updatedTemplateMaster.Id);
            if (existingTemplateMaster == null)
            {
                return new ServiceResponse<TemplateMaster>
                {
                    Success = false,
                    Message = "Template master not found"
                };
            }

            existingTemplateMaster.Name = updatedTemplateMaster.Name;
            // Update other properties as needed
            _context.TemplateMasters.Update(existingTemplateMaster);
            await _context.SaveChangesAsync();

            return new ServiceResponse<TemplateMaster>
            {
                Data = existingTemplateMaster,
                Success = true,
                Message = "Template master updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating template master.");
            return new ServiceResponse<TemplateMaster>
            {
                Success = false,
                Message = "Error occurred while updating template master"
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteTemplateMasterAsync(long id)
    {
        try
        {
            var templateMaster = await _context.TemplateMasters.FindAsync(id);
            if (templateMaster == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Template master not found"
                };
            }

            _context.TemplateMasters.Remove(templateMaster);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Template master deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting template master.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting template master"
            };
        }
    }
}