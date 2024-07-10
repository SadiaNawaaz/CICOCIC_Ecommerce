using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.Users;

public interface IUserService
{
    Task<ServiceResponse<List<User>>> GetUsersAsync();
    Task<ServiceResponse<User>> GetUserByIdAsync(long id);
    Task<ServiceResponse<User>> AddUserAsync(User user);
    Task<ServiceResponse<User>> UpdateUserAsync(User user);
    Task<ServiceResponse<bool>> DeleteUserAsync(long id);
    Task<ServiceResponse<User>> LoginAsync(string email, string password);
}
public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<User>>> GetUsersAsync()
    {
        try
        {
            var users = await _context.Users.Include(a=>a.Role).ToListAsync();
            return new ServiceResponse<List<User>>
            {
                Data = users,
                Success = true,
                Message = "Users fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching users.");
            return new ServiceResponse<List<User>>
            {
                Success = false,
                Message = "Error occurred while fetching users"
            };
        }
    }

    public async Task<ServiceResponse<User>> GetUserByIdAsync(long id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            return new ServiceResponse<User>
            {
                Data = user,
                Success = true,
                Message = "User fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching user by ID.");
            return new ServiceResponse<User>
            {
                Success = false,
                Message = "Error occurred while fetching user by ID"
            };
        }
    }

    public async Task<ServiceResponse<User>> AddUserAsync(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new ServiceResponse<User>
            {
                Data = user,
                Success = true,
                Message = "User added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding user.");
            return new ServiceResponse<User>
            {
                Success = false,
                Message = "Error occurred while adding user"
            };
        }
    }

    public async Task<ServiceResponse<User>> UpdateUserAsync(User updatedUser)
    {
        try
        {
            var existingUser = await _context.Users.FindAsync(updatedUser.Id);

            if (existingUser == null)
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Email = updatedUser.Email;
            existingUser.Password = updatedUser.Password; // Consider hashing the password

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            return new ServiceResponse<User>
            {
                Data = existingUser,
                Success = true,
                Message = "User updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user.");
            return new ServiceResponse<User>
            {
                Success = false,
                Message = "Error occurred while updating user"
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteUserAsync(long id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "User deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting user"
            };
        }
    }

    public async Task<ServiceResponse<User>> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _context.Users.Include(a=>a.Role).FirstOrDefaultAsync(u => u.Email == email && u.Password == password); // Consider hashing the password for comparison

            if (user == null)
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            return new ServiceResponse<User>
            {
                Data = user,
                Success = true,
                Message = "Login successful"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging in.");
            return new ServiceResponse<User>
            {
                Success = false,
                Message = "Error occurred while logging in"
            };
        }
    }
}
