using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.Customers;


public interface ICustomerService
    {
    Task<ServiceResponse<Customer>> AddCustomerAsync(Customer customer);
    Task<ServiceResponse<Customer>> ValidateUserAsync(string email, string password);
    }

public class CustomerService : ICustomerService
    {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(ApplicationDbContext context, ILogger<CustomerService> logger)
        {
        _context = context;
        _logger = logger;
        }

    public async Task<ServiceResponse<Customer>> AddCustomerAsync(Customer customer)
        {
        try
            {
            var existingCustomer = await _context.Customers
                .AnyAsync(c => c.Email == customer.Email);

            if (existingCustomer)
                {
                return new ServiceResponse<Customer>
                    {
                    Success = false,
                    Message = "Email already exists."
                    };
                }

            // Hash the password before storing it
            customer.Password = PasswordHasher.HashPassword(customer.Password);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Customer>
                {
                Data = customer,
                Success = true,
                Message = "Customer added successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while adding customer.");
            return new ServiceResponse<Customer>
                {
                Success = false,
                Message = "Error occurred while adding customer"
                };
            }
        }

    public async Task<ServiceResponse<Customer>> ValidateUserAsync(string email, string password)
        {
        var response = new ServiceResponse<Customer>();
        try
            {
            var user = await _context.Customers.SingleOrDefaultAsync(c => c.Email == email);

            if (user == null)
                {
                response.Success = false;
                response.Message = "User not found.";
                return response;
                }

            // Verify hashed password
            var passwordValid = PasswordHasher.VerifyPassword(user.Password, password);

            if (passwordValid)
                {
                response.Success = true;
                response.Data = user; // Return user if validation is successful
                }
            else
                {
                response.Success = false;
                response.Message = "Invalid password.";
                }
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while validating user.");
            response.Success = false;
            response.Message = "An error occurred while validating the user.";
            }

        return response;
        }
    }

public class PasswordHasher
    {
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32;  // 256 bit
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

    public static string HashPassword(string password)
        {
        using (var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithm))
            {
            var salt = algorithm.Salt;
            var key = algorithm.GetBytes(KeySize);

            var hashBytes = new byte[SaltSize + KeySize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(key, 0, hashBytes, SaltSize, KeySize);

            return Convert.ToBase64String(hashBytes);
            }
        }

    public static bool VerifyPassword(string hashedPassword, string providedPassword)
        {
        var hashBytes = Convert.FromBase64String(hashedPassword);

        var salt = new byte[SaltSize];
        Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

        var key = new byte[KeySize];
        Buffer.BlockCopy(hashBytes, SaltSize, key, 0, KeySize);

        using (var algorithm = new Rfc2898DeriveBytes(providedPassword, salt, Iterations, HashAlgorithm))
            {
            var keyToCheck = algorithm.GetBytes(KeySize);
            return keyToCheck.SequenceEqual(key);  // Returns true if passwords match
            }
        }
    }
