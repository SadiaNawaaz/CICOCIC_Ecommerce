﻿using Ecommerce.Shared.Context;
using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Entities.Catalogs;
using Ecommerce.Shared.Entities.Colors;
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
    Task<ServiceResponse<List<ProductDto>>> GetProductsWithVariantsAsync();
    Task<ServiceResponse<long?>> GetMinimumVariantPriceAsync(long productId);

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
            var product = await _context.Products
           .Include(c => c.Brand)                   
           .Include(c => c.Category)
           .Include(c => c.ProductImages)
           .Include(c=>c.Translations)
           .Include(c => c.ProductMedias)
           .Include(c => c.ProductClusters)         
               .ThenInclude(cc => cc.Cluster)       
           .Include(c => c.ProductClusters)         
               .ThenInclude(cc => cc.ProductClusterFeatures)  
                   .ThenInclude(cf => cf.Feature)   
           .FirstOrDefaultAsync(c => c.Id == id);
            return new ServiceResponse<Product>
            {
                Data = product,
                Success = true,
                Message = "Catalog fetched successfully"
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

        //product.ProductClusters.RemoveAll(a => a.Id == 0);
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
            {
            // Add product to the database


            var invalidFeatureIds = product.ProductClusters?
          .SelectMany(pc => pc.ProductClusterFeatures)
          .Where(f => !_context.Features.Any(fe => fe.Id == f.FeatureId))
          .Select(f => f.FeatureId)
          .ToList();


            _context.Products.Add(product);
            var catalog = await _context.Catalogs.FindAsync(product.CatalogId);
            await _context.SaveChangesAsync();

            if (catalog != null && catalog.Integrated == true)
                {
                // Extract Color from FeatureValue of FeatureId 1766
                var featureWithColor = product.ProductClusters?
                    .SelectMany(pc => pc.ProductClusterFeatures)
                    .FirstOrDefault(f => f.FeatureId == 1766);

                if (featureWithColor != null && !string.IsNullOrEmpty(featureWithColor.Value))
                    {           
                    var color = featureWithColor.Value.Split(',').FirstOrDefault()?.Trim();
                    var existingColor = await _context.Colors.FirstOrDefaultAsync(c => c.Name == color);

                    if (existingColor == null)
                        {
                        var newColor = new GeneralColor { Name = color,HexCode="" };
                        _context.Colors.Add(newColor);
                        await _context.SaveChangesAsync();
                        catalog.GeneralColorId = newColor.Id;
                        }
                    else
                        {
                        catalog.GeneralColorId = existingColor.Id;
                        }
                    }
                catalog.MarkProduct = true;
                await _context.SaveChangesAsync();
                }

            // Commit transaction
            await transaction.CommitAsync();

            return new ServiceResponse<Product>
                {
                Data = product,
                Success = true,
                Message = "Product added successfully"
                };
            }
        catch (Exception ex)
            {
            // Rollback transaction in case of an error
            await transaction.RollbackAsync();

            _logger.LogError(ex, $"Error occurred while adding product. {ex.InnerException}");
            return new ServiceResponse<Product>
                {
                Success = false,
                InnerException = ex.InnerException?.ToString(),
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
            //existingProduct.TemplateMasterId = updatedProduct.TemplateMasterId;
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
            _logger.LogError(ex, "Error occurred while updating product." + ex.Message);
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


    public async Task<ServiceResponse<List<ProductDto>>> GetProductsWithVariantsAsync()
    {
        try
        {
            FormattableString sql = $@"
               SELECT 
                p.Id AS Id,
                p.Name AS Name,
                p.Price,
               b.Name AS Brand,
                c.Name AS Category,p.Thumbnail,
                COUNT(pv.Id) AS Stock
                    FROM 
                     Products p
                     LEFT JOIN 
                     ProductVariants pv ON p.Id = pv.ProductId AND (ISNULL(pv.Sold, 0) = 0)
                    LEFT JOIN 
                    Brands b ON p.BrandId = b.Id
                     LEFT JOIN 
                      Categories c ON p.CategoryId = c.Id
                      GROUP BY 
                       p.Id, p.Name, b.Name, c.Name,p.Price,p.Thumbnail
                         ";


            var products = await _context.Database.SqlQuery<ProductDto>(sql).ToListAsync();
            return new ServiceResponse<List<ProductDto>>
            {
                Data = products,
                Success = true,
                Message = "Products and variants fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products and variants.");
            return new ServiceResponse<List<ProductDto>>
            {
                Success = false,
                Message = "Error occurred while fetching products and variants"
            };
        }
    }

    public async Task<ServiceResponse<long?>> GetMinimumVariantPriceAsync(long productId)
    {
        try
        {
            var minPrice = await _context.ProductVariants
                                         .Where(v => v.ProductId == productId)
                                         .MinAsync(v => (long?)v.Price);

            return new ServiceResponse<long?>
            {
                Data = minPrice,
                Success = true,
                Message = "Minimum variant price fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching minimum variant price.");
            return new ServiceResponse<long?>
            {
                Success = false,
                Message = "Error occurred while fetching minimum variant price"
            };
        }
    }

}