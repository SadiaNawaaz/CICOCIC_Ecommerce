using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Permissions;
using Ecommerce.Shared.Entities.Roles;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.RolePermissions;

public interface IRolePermissionService
{
    Task<ServiceResponse<List<Permission>>> GetPermissionsForRoleAsync(long roleId);
    Task<ServiceResponse<bool>> AssignPermissionsToRoleAsync(long roleId, List<PermissionDto> permissions);
    Task<ServiceResponse<bool>> RemovePermissionsFromRoleAsync(long roleId, List<long> permissionIds);
    Task<ServiceResponse<List<PermissionDto>>> GetAllPermissionsAsync();

}

public class RolePermissionService : IRolePermissionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RolePermissionService> _logger;

    public RolePermissionService(ApplicationDbContext context, ILogger<RolePermissionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Permission>>> GetPermissionsForRoleAsync(long roleId)
    {
        try
        {
            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission)
                .ToListAsync();

            return new ServiceResponse<List<Permission>>
            {
                Data = permissions,
                Success = true,
                Message = "Permissions fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching permissions for role.");
            return new ServiceResponse<List<Permission>>
            {
                Success = false,
                Message = "Error occurred while fetching permissions"
            };
        }
    }

    public async Task<ServiceResponse<bool>> AssignPermissionsToRoleAsync(long roleId, List<PermissionDto> permissions)
    {
        try
        {
            var role = await _context.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Role not found"
                };
            }

            // Clear existing permissions
            role.RolePermissions.Clear();

            // Add new permissions
            foreach (var permission in permissions)
            {
                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.Id
                });
            }

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Permissions assigned successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while assigning permissions to role.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while assigning permissions"
            };
        }
    }

    public async Task<ServiceResponse<bool>> RemovePermissionsFromRoleAsync(long roleId, List<long> permissionIds)
    {
        try
        {
            var role = await _context.Roles.Include(r => r.RolePermissions)
                                           .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Role not found"
                };
            }

            // Remove the specified permissions
            role.RolePermissions.RemoveAll(rp => permissionIds.Contains(rp.PermissionId));

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Permissions removed successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing permissions from role.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while removing permissions"
            };
        }
    }

    public async Task<ServiceResponse<List<PermissionDto>>> GetAllPermissionsAsync()
    {
        try
        {
            var permissions = await _context.Permissions.ToListAsync();

            // Map Permission entities to PermissionDto
            var permissionDtos = permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                ActionName = p.ActionName,
                PageUrl = p.PageUrl,
                Description = p.Description,
                ParentId = p.ParentId
            }).ToList();

            return new ServiceResponse<List<PermissionDto>>
            {
                Data = permissionDtos,
                Success = true,
                Message = "Permissions fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all permissions.");
            return new ServiceResponse<List<PermissionDto>>
            {
                Success = false,
                Message = "Error occurred while fetching permissions"
            };
        }
    }

}


