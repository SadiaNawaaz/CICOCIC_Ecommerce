using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Products;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Ecommerce.Shared.Services.Products;
public interface IProductService
{
    Task<ServiceResponse<List<Product>>> GetProductsAsync();
    Task<ServiceResponse<Product>> GetProductByIdAsync(long id);
    Task<ServiceResponse<Product>> AddProductAsync(Product product);
    Task<ServiceResponse<Product>> UpdateProductAsync(Product product);
    Task<ServiceResponse<bool>> DeleteProductAsync(long id);
    Task<ServiceResponse<List<Product>>> GetProductsByCategoryIdAsync(long CategoryId);
}

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
    {
        try
        {
            var products = await _context.Products
                               .Include(p => p.Category)
                               .Include(p => p.TemplateMaster)
                               .ToListAsync();
            return new ServiceResponse<List<Product>>
            {
                Data = products,
                Success = true,
                Message = "Products fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products.");
            return new ServiceResponse<List<Product>>
            {
                Success = false,
                Message = "Error occurred while fetching products"
            };
        }
    }

    public async Task<ServiceResponse<Product>> GetProductByIdAsync(long id)
    {
        try
        {
            var product = await _context.Products.Include(p => p.FeatureValues).FirstOrDefaultAsync(p => p.Id == id);

            return new ServiceResponse<Product>
            {
                Data = product,
                Success = true,
                Message = "Product fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching product by ID.");
            return new ServiceResponse<Product>
            {
                Success = false,
                Message = "Error occurred while fetching product by ID"
            };
        }
    }

    public async Task<ServiceResponse<Product>> AddProductAsync(Product product)
    {
        try
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return new ServiceResponse<Product>
            {
                Data = product,
                Success = true,
                Message = "Product added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding product.");
            return new ServiceResponse<Product>
            {
                Success = false,
                Message = "Error occurred while adding product"
            };
        }
    }

    public async Task<ServiceResponse<Product>> UpdateProductAsync(Product updatedProduct)
    {
        try
        {
            var existingProduct = await _context.Products.FindAsync(updatedProduct.Id);

            if (existingProduct == null)
            {
                return new ServiceResponse<Product>
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            existingProduct.Name = updatedProduct.Name;
            existingProduct.CategoryId = updatedProduct.CategoryId;
            existingProduct.TemplateMasterId = updatedProduct.TemplateMasterId;
            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Product>
            {
                Data = existingProduct,
                Success = true,
                Message = "Product updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product.");
            return new ServiceResponse<Product>
            {
                Success = false,
                Message = "Error occurred while updating product"
            };
        }
    }

    public async Task<ServiceResponse<bool>> DeleteProductAsync(long id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Product deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting product.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting product"
            };
        }
    }



    public async Task<ServiceResponse<List<Product>>> GetProductsByCategoryIdAsync(long CategoryId)
    {
        try
        {
            var products = await _context.Products.Where(a=>a.CategoryId== CategoryId).ToListAsync();
            return new ServiceResponse<List<Product>>
            {
                Data = products,
                Success = true,
                Message = "Products fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products.");
            return new ServiceResponse<List<Product>>
            {
                Success = false,
                Message = "Error occurred while fetching products"
            };
        }
    }

}