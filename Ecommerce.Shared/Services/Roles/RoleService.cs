using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Roles;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.Roles;

public interface IRoleService
{
    Task<ServiceResponse<List<Role>>> GetRolesAsync();
    Task<ServiceResponse<Role>> GetRoleByIdAsync(long id);
    Task<ServiceResponse<Role>> AddRoleAsync(Role role);
    Task<ServiceResponse<Role>> UpdateRoleAsync(Role role);
    Task<ServiceResponse<bool>> DeleteRoleAsync(long id);
}

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoleService> _logger;

    public RoleService(ApplicationDbContext context, ILogger<RoleService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Role>>> GetRolesAsync()
    {
        try
        {
            var roles = await _context.Roles.ToListAsync();
            return new ServiceResponse<List<Role>>
            {
                Data = roles,
                Success = true,
                Message = "Roles fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching roles.");
            return new ServiceResponse<List<Role>>
            {
                Success = false,
                Message = "Error occurred while fetching roles"
            };
        }
    }

    public async Task<ServiceResponse<Role>> GetRoleByIdAsync(long id)
    {
        try
        {
            var role = await _context.Roles.FindAsync(id);
            return new ServiceResponse<Role>
            {
                Data = role,
                Success = true,
                Message = "Role fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching role by ID.");
            return new ServiceResponse<Role>
            {
                Success = false,
                Message = "Error occurred while fetching role by ID"
            };
        }
    }

    public async Task<ServiceResponse<Role>> AddRoleAsync(Role role)
    {
        try
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return new ServiceResponse<Role>
            {
                Data = role,
                Success = true,
                Message = "Role added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding role.");
            return new ServiceResponse<Role>
            {
                Success = false,
                Message = "Error occurred while adding role"
            };
        }
    }

    public async Task<ServiceResponse<Role>> UpdateRoleAsync(Role updatedRole)
    {
        try
        {
            var existingRole = await _context.Roles.FindAsync(updatedRole.Id);

            if (existingRole == null)
            {
                return new ServiceResponse<Role>
                {
                    Success = false,
                    Message = "Role not found"
                };
            }

            existingRole.Name = updatedRole.Name;
            existingRole.Description = updatedRole.Description;
            _context.Roles.Update(existingRole);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Role>
            {
                Data = existingRole,
                Success = true,
                Message = "Role updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating role.");
            return new ServiceResponse<Role>
            {
                Success = false,
                Message = "Error occurred while updating role"
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteRoleAsync(long id)
    {
        try
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Role not found"
                };
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Role deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting role.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting role"
            };
        }
    }
}