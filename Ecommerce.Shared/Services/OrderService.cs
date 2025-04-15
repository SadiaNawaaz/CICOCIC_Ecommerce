using Ecommerce.Shared.Context;
using Ecommerce.Shared.Dto;
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
    Task<ServiceResponse<List<OrderDto>>> GetOrdersByCustomerId(long customerId);
    Task<ServiceResponse<bool>> UpdateOrderStatus(long orderId, OrderStatus newStatus);
    Task<ServiceResponse<OrderDto>> GetOrderDtoById(long orderId);

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
        using (var transaction = await _context.Database.BeginTransactionAsync())
            {
            try
                {
                order.OrderDate = DateTime.Now;
                await _context.Orders.AddAsync(order);
                foreach (var orderItem in order.OrderItems)
                    {
                    var product = await _context.ProductVariants.FirstOrDefaultAsync(p => p.Id == orderItem.VariantId);

                    if (product == null)
                        {
                        throw new Exception($"Product with ID {orderItem.ProductId} not found.");
                        }
                    //if (product.Sold == 1)
                    //    {
                    //    throw new Exception($"Product with ID {product.Id} is already sold.");
                    //    }
                    product.Sold = 1;
                    product.Publish = false;
                    _context.ProductVariants.Update(product);
                    }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ServiceResponse<Order>
                    {
                    Data = order,
                    Success = true,
                    Message = "Order placed successfully"
                    };
                }
            catch (Exception ex)
                {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while placing order.");

                return new ServiceResponse<Order>
                    {
                    Success = false,
                    Message = ex.Message 
                    };
                }
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

    public async Task<ServiceResponse<List<OrderDto>>> GetOrdersByCustomerId(long customerId)
        {
        try
            {
            // Fetch orders and related order items
            var orders = await _context.Orders
                .Include(o => o.OrderItems) // Include OrderItems
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            // Map orders and order items to DTOs
            var orderDtos = orders.Select(order => new OrderDto
                {
                Id = order.Id,
                CustomerId = order.CustomerId,
                FirstName = order.FirstName,
                LastName = order.LastName,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                    {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    VariantName = _context.ProductVariants.FirstOrDefault(pv => pv.Id == item.VariantId)?.Name ?? "N/A", // Fetch VariantName directly
                    Price = item.Price,
                    Quantity = item.Quantity,
                    DefaultImageUrl = _context.ProductVariants.FirstOrDefault(pv => pv.Id == item.VariantId)?.Thumbnail ?? "N/A"
                    }).ToList()
                }).ToList();

            return new ServiceResponse<List<OrderDto>>
                {
                Data = orderDtos,
                Success = true,
                Message = "Orders retrieved successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while retrieving orders.");
            return new ServiceResponse<List<OrderDto>>
                {
                Success = false,
                Message = "Error occurred while retrieving orders"
                };
            }
        }


    public async Task<ServiceResponse<bool>> UpdateOrderStatus(long orderId, OrderStatus newStatus)
        {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
            {
            return new ServiceResponse<bool> { Success = false, Message = "Order not found." };
            }

        order.Status = newStatus;
        await _context.SaveChangesAsync();

        return new ServiceResponse<bool> { Success = true, Data = true };
        }




    public async Task<ServiceResponse<OrderDto>> GetOrderDtoById(long orderId)
        {
        try
            {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                {
                return new ServiceResponse<OrderDto>
                    {
                    Success = false,
                    Message = "Order not found"
                    };
                }

            // Retrieve the product variants related to the order items
            var variantIds = order.OrderItems.Select(oi => oi.VariantId).ToList();
            var variants = await _context.ProductVariants
                .Where(pv => variantIds.Contains(pv.Id))
                .ToDictionaryAsync(pv => pv.Id);

            // Map the order entity to the OrderDto
            var orderDto = new OrderDto
                {
                Id = order.Id,
                CustomerId = order.CustomerId,
                FirstName = order.FirstName,
                LastName = order.LastName,
                TotalAmount = order.TotalAmount,
                OrderNumber=order.OrderNumber,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                    {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    VariantName = variants.TryGetValue(item.VariantId, out var variant) ? variant.Name : "N/A",
                    Price = item.Price,
                    Quantity = item.Quantity,
                    DefaultImageUrl = variants.TryGetValue(item.VariantId, out variant) ? variant.Thumbnail : "N/A"
                    }).ToList()
                };

            return new ServiceResponse<OrderDto>
                {
                Data = orderDto,
                Success = true,
                Message = "Order retrieved successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while retrieving the order.");
            return new ServiceResponse<OrderDto>
                {
                Success = false,
                Message = "Error occurred while retrieving the order"
                };
            }
        }

    }