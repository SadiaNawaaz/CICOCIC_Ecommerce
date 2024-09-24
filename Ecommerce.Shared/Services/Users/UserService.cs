using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Services.Shared;
using Microsoft.Data.SqlClient;
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
    Task<ServiceResponse<List<User>>> GetUsersAsync(bool IsAgent = false);
    Task<ServiceResponse<User>> GetUserByIdAsync(long id);
    Task<ServiceResponse<User>> AddUserAsync(User user);
    Task<ServiceResponse<User>> UpdateUserAsync(User user);
    Task<ServiceResponse<bool>> DeleteUserAsync(long id);
    Task<ServiceResponse<User>> LoginAsync(string email, string password);

    Task<ServiceResponse<User>> UpdateAgentStatus(User user);
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

    public async Task<ServiceResponse<List<User>>> GetUsersAsync(bool IsAgent=false)
    {
        try
        {
           
            if(IsAgent)
            {
                var users = await _context.Users.Include(a=>a.UserRoles).Where(a=>a.IsAgent==true).ToListAsync();
                return new ServiceResponse<List<User>>
                {
                    Data = users,
                    Success = true,
                    Message = "Agents fetched successfully"
                };
            }
            else
            {
                var users = await _context.Users.Include(a => a.UserRoles).ToListAsync();
                return new ServiceResponse<List<User>>
                {
                    Data = users,
                    Success = true,
                    Message = "Users fetched successfully"
                };
            }
          
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

            var existingUser = await _context.Users
        .AnyAsync(u => u.UserName == user.UserName || u.Email == user.Email);

            if (existingUser)
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "Username or email already exists."
                };
            }
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

    public async Task<ServiceResponse<User>> LoginAsync(string Username, string password)
    {
        try
        {
            //string sourceConnectionString = "Server=mssql008.db.hosting;Database=ID432610_cico;User Id=ID432610_cico;Password=Gazelle11;Integrated Security=false;Trusted_Connection=false;MultipleActiveResultSets=true;Encrypt=false;";

            //// Destination server connection details
            //string destinationConnectionString = "Server=mssql005.db.hosting;Database=ID432610_cicocic;User Id=ID432610_cicocic;Password=Gazelle11;Integrated Security=false;Trusted_Connection=false;MultipleActiveResultSets=true;Encrypt=false;";

            //// Source table and destination table names
            //string sourceTable = "tmpCatalogszero";
            //string destinationTable = "tmpCatalogs"; // Replace with the actual destination table name

            //// SQL query to select data from the source table
            //string selectQuery = $"SELECT [Id], [Name], [Slug], [BrandId], [Code], [Thumbnail], [Description], [ShortDescription], [Price], [CategoryId], [Media], [ColorId], [SizeId], [Year] FROM {sourceTable}";

            //// SQL query to insert data into the destination table
            //string insertQuery = $@"
            //INSERT INTO {destinationTable} (
            //    Id, Name, Slug, BrandId, Code, Thumbnail, Description, ShortDescription, Price, CategoryId, Media, ColorId, SizeId, Year
            //) VALUES (
            //    @Id, @Name, @Slug, @BrandId, @Code, @Thumbnail, @Description, @ShortDescription, @Price, @CategoryId, @Media, @ColorId, @SizeId, @Year
            //)";
            //try
            //    {
            //    // Connect to the source server and fetch data
            //    using (SqlConnection sourceConnection = new SqlConnection(sourceConnectionString))
            //        {
            //        sourceConnection.Open();
            //        using (SqlCommand selectCommand = new SqlCommand(selectQuery, sourceConnection))
            //            {
            //            using (SqlDataReader reader = selectCommand.ExecuteReader())
            //                {
            //                // Connect to the destination server and insert data
            //                using (SqlConnection destinationConnection = new SqlConnection(destinationConnectionString))
            //                    {
            //                    destinationConnection.Open();
            //                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, destinationConnection))
            //                        {
            //                        insertCommand.Parameters.Add("@Id", System.Data.SqlDbType.Int);
            //                        insertCommand.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
            //                        insertCommand.Parameters.Add("@Slug", System.Data.SqlDbType.NVarChar);
            //                        insertCommand.Parameters.Add("@BrandId", System.Data.SqlDbType.Int);
            //                        insertCommand.Parameters.Add("@Code", System.Data.SqlDbType.NVarChar);
            //                        insertCommand.Parameters.Add("@Thumbnail", System.Data.SqlDbType.NVarChar);
            //                        insertCommand.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar);
            //                        insertCommand.Parameters.Add("@ShortDescription", System.Data.SqlDbType.NVarChar);
            //                        insertCommand.Parameters.Add("@Price", System.Data.SqlDbType.Decimal);
            //                        insertCommand.Parameters.Add("@CategoryId", System.Data.SqlDbType.Int);
            //                        insertCommand.Parameters.Add("@Media", System.Data.SqlDbType.NVarChar);
            //                        insertCommand.Parameters.Add("@ColorId", System.Data.SqlDbType.Int);
            //                        insertCommand.Parameters.Add("@SizeId", System.Data.SqlDbType.Int);
            //                        insertCommand.Parameters.Add("@Year", System.Data.SqlDbType.Int);

            //                        // Iterate through each row in the source data
            //                        while (reader.Read())
            //                            {
            //                            // Set parameter values
            //                            insertCommand.Parameters["@Id"].Value = reader["Id"];
            //                            insertCommand.Parameters["@Name"].Value = reader["Name"];
            //                            insertCommand.Parameters["@Slug"].Value = reader["Slug"];
            //                            insertCommand.Parameters["@BrandId"].Value = reader["BrandId"];
            //                            insertCommand.Parameters["@Code"].Value = reader["Code"];
            //                            insertCommand.Parameters["@Thumbnail"].Value = reader["Thumbnail"];
            //                            insertCommand.Parameters["@Description"].Value = reader["Description"];
            //                            insertCommand.Parameters["@ShortDescription"].Value = reader["ShortDescription"];
            //                            insertCommand.Parameters["@Price"].Value = reader["Price"];
            //                            insertCommand.Parameters["@CategoryId"].Value = reader["CategoryId"];
            //                            insertCommand.Parameters["@Media"].Value = reader["Media"];
            //                            insertCommand.Parameters["@ColorId"].Value = reader["ColorId"];
            //                            insertCommand.Parameters["@SizeId"].Value = reader["SizeId"];
            //                            insertCommand.Parameters["@Year"].Value = reader["Year"];

            //                            // Execute the insert command
            //                            insertCommand.ExecuteNonQuery();
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //    Console.WriteLine("Data transfer completed successfully.");
            //    }
            //catch (Exception ex)
            //    {
            //    Console.WriteLine($"An error occurred: {ex.Message}");
            //    }
            var user = await _context.Users
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role) 
              .ThenInclude(r => r.RolePermissions) 
          .FirstOrDefaultAsync(u => u.UserName == Username && u.Password == password); 
 
            if (user == null)
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "Invalid Username or password"
                };
            }
            if (user.IsAgent==true && user.Approved==false)
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "You're not approved. Please wait for your request to be approved by an admin."
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

    public async Task<ServiceResponse<User>> UpdateAgentStatus(User updatedUser)
    {
        try
        {
            var existingUser = await _context.Users.FindAsync(updatedUser.Id);

            if (existingUser == null)
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "Agent not found"
                };
            }
            existingUser.Approved = updatedUser.Approved;
            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            return new ServiceResponse<User>
            {
                Data = existingUser,
                Success = true,
                Message = "Agent Status Updated successfully"
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
}
