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

namespace Ecommerce.Shared.Services.Customers;


    public interface ICustomerService
        {
        Task<ServiceResponse<Customer>> AddCustomerAsync(Customer customer);
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
                // Check if the customer already exists by email
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

                // Add the customer to the database
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
        }

