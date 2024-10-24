using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Orders;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services;

public interface IOrderService
    {
    Task<ServiceResponse<Order>> PlaceOrder(Order order);
    Task<ServiceResponse<Order>> GetOrderById(long orderId);
    Task<ServiceResponse<List<Order>>> GetOrdersByCustomerId(long customerId);
    }
public class OrderService : IOrderService
    {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderService> _logger;

    public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
        {
        _context = context;
        _logger = logger;
        }

    public async Task<ServiceResponse<Order>> PlaceOrder(Order order)
        {
        try
            {
            order.OrderDate = DateTime.Now;
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Order>
                {
                Data = order,
                Success = true,
                Message = "Order placed successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while placing order.");
            return new ServiceResponse<Order>
                {
                Success = false,
                Message = "Error occurred while placing order"
                };
            }
        }

    public async Task<ServiceResponse<Order>> GetOrderById(long orderId)
        {
        try
            {
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                {
                return new ServiceResponse<Order>
                    {
                    Success = false,
                    Message = "Order not found"
                    };
                }

            return new ServiceResponse<Order>
                {
                Data = order,
                Success = true,
                Message = "Order retrieved successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while retrieving order.");
            return new ServiceResponse<Order>
                {
                Success = false,
                Message = "Error occurred while retrieving order"
                };
            }
        }

    public async Task<ServiceResponse<List<Order>>> GetOrdersByCustomerId(long customerId)
        {
        try
            {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            return new ServiceResponse<List<Order>>
                {
                Data = orders,
                Success = true,
                Message = "Orders retrieved successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while retrieving orders.");
            return new ServiceResponse<List<Order>>
                {
                Success = false,
                Message = "Error occurred while retrieving orders"
                };
            }
        }
    }