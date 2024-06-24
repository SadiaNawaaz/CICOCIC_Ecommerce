using Ecommerce.Shared.Context;
using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Entities.ProductVariants;
using Ecommerce.Shared.Services.Products;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.ProductVariants;


public interface IProductVariantService
{

    Task<ServiceResponse<ProductVariant>> AddProductVariantAsync(ProductVariant productVariant);
    Task<ServiceResponse<ProductVariant>> UpdateProductVariantAsync(ProductVariant productVariant);
    Task<ServiceResponse<bool>> DeleteProductVariantAsync(long id);
    Task<ServiceResponse<ProductVariant>> GetProductVariantByIdAsync(long id);
    Task<ServiceResponse<List<ProductVariant>>> GetAllProductVariantsAsync(long ProductId);
    Task<ServiceResponse<List<ProductVariantDto>>> GetProductVariantsByCategoryAsync(long categoryId);
}

public class ProductVariantService: IProductVariantService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductVariantService(ApplicationDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<ProductVariant>> AddProductVariantAsync(ProductVariant productVariant)
    {
        try
        {
            _context.ProductVariants.Add(productVariant);
            await _context.SaveChangesAsync();
            return new ServiceResponse<ProductVariant>
            {
                Data = productVariant,
                Success = true,
                Message = "Product variant added successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product variant.");
            return new ServiceResponse<ProductVariant>
            {
                Success = false,
                Message = $"An error occurred while adding the product variant: {ex.Message}"
            };
        }
    }

    public async Task<ServiceResponse<ProductVariant>> UpdateProductVariantAsync1(ProductVariant productVariant)
    {
        try
        {
            var existingVariant = await _context.ProductVariants.FindAsync(productVariant.Id);
            if (existingVariant == null)
            {
                return new ServiceResponse<ProductVariant>
                {
                    Success = false,
                    Message = "Product variant not found."
                };
            }



         
            await _context.SaveChangesAsync();
            return new ServiceResponse<ProductVariant>
            {
                Data = existingVariant,
                Success = true,
                Message = "Product variant updated successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product variant.");
            return new ServiceResponse<ProductVariant>
            {
                Success = false,
                Message = $"An error occurred while updating the product variant: {ex.Message}"
            };
        }
    }
    public async Task<ServiceResponse<ProductVariant>> UpdateProductVariantAsync(ProductVariant updatedProductVariant)
    {
        try
        {
            // Retrieve the existing product variant with related entities
            var existingProductVariant = await _context.ProductVariants
                .Include(pv => pv.ProductVariantFeatureValues)
                .FirstOrDefaultAsync(pv => pv.Id == updatedProductVariant.Id);

            if (existingProductVariant == null)
            {
                return new ServiceResponse<ProductVariant>
                {
                    Success = false,
                    Message = "Product variant not found"
                };
            }

            // Update scalar properties
            existingProductVariant.Name = updatedProductVariant.Name;
            existingProductVariant.ProductId = updatedProductVariant.ProductId;
            existingProductVariant.GeneralSizeId = updatedProductVariant.GeneralSizeId;
            existingProductVariant.GeneralColorId = updatedProductVariant.GeneralColorId;
            existingProductVariant.Price = updatedProductVariant.Price;
            existingProductVariant.Sku = updatedProductVariant.Sku;
            existingProductVariant.Value = updatedProductVariant.Value;
            existingProductVariant.SSN = updatedProductVariant.SSN;
            existingProductVariant.Description = updatedProductVariant.Description;
            existingProductVariant.ModelYearId = updatedProductVariant.ModelYearId;
            existingProductVariant.Publish = updatedProductVariant.Publish;



            // Save changes
            _context.ProductVariants.Update(existingProductVariant);
            await _context.SaveChangesAsync();

            return new ServiceResponse<ProductVariant>
            {
                Data = existingProductVariant,
                Success = true,
                Message = "Product variant updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product variant.");
            return new ServiceResponse<ProductVariant>
            {
                Success = false,
                Message = "Error occurred while updating product variant"
            };
        }
    }
    public async Task<ServiceResponse<bool>> DeleteProductVariantAsync(long id)
    {
        try
        {
            var productVariant = await _context.ProductVariants.FindAsync(id);
            if (productVariant == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Product variant not found."
                };
            }

            _context.ProductVariants.Remove(productVariant);
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Product variant deleted successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product variant.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = $"An error occurred while deleting the product variant: {ex.Message}"
            };
        }
    }

    public async Task<ServiceResponse<ProductVariant>> GetProductVariantByIdAsync(long id)
    {
        try
        {
            var productVariant = await _context.ProductVariants
                .Include(pv => pv.GeneralSize)
                .Include(pv => pv.GeneralColor)
                .Include(pv => pv.ModelYear)
                .Include(fv => fv.ProductVariantFeatureValues)
                .Include(img => img.productVariantImages)
                .FirstOrDefaultAsync(pv => pv.Id == id);

            if (productVariant == null)
            {
                return new ServiceResponse<ProductVariant>
                {
                    Success = false,
                    Message = "Product variant not found."
                };
            }

            return new ServiceResponse<ProductVariant>
            {
                Data = productVariant,
                Success = true,
                Message = "Product variant fetched successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product variant.");
            return new ServiceResponse<ProductVariant>
            {
                Success = false,
                Message = $"An error occurred while fetching the product variant: {ex.Message}"
            };
        }
    }

    public async Task<ServiceResponse<List<ProductVariant>>> GetAllProductVariantsAsync(long ProductId)
    {
        try
        {
            var productVariants = await _context.ProductVariants
                .Include(pv => pv.GeneralSize)
                .Include(pv => pv.GeneralColor)
                .Include(pv => pv.ModelYear).Where(a=>a.ProductId== ProductId)
                .ToListAsync();

            return new ServiceResponse<List<ProductVariant>>
            {
                Data = productVariants,
                Success = true,
                Message = "Product variants fetched successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product variants.");
            return new ServiceResponse<List<ProductVariant>>
            {
                Success = false,
                Message = $"An error occurred while fetching product variants: {ex.Message}"
            };
        }
    }

    public async Task<ServiceResponse<List<ProductVariantDto>>> GetProductVariantsByCategoryAsync(long categoryId)
    {
        var response = new ServiceResponse<List<ProductVariantDto>>();

        try
        {
            var productVariants = await _context.ProductVariants
                .Where(pv => pv.Product.CategoryId == categoryId)
                .Select(pv => new ProductVariantDto
                {
                    ProductId = pv.ProductId,
                    Id = pv.Id,
                    Name = pv.Product.Name,
                    Category = pv.Product.Category.Name,
                    Color = pv.GeneralColor != null ? pv.GeneralColor.Name : "No Color",
                    Size = pv.GeneralSize != null ? pv.GeneralSize.Name : "No Size",
                    VariantPrice = pv.Price,
                    ProductPrice = pv.Product.Price,
                    Sku = pv.Sku,
                    DefaultImageUrl = pv.productVariantImages.FirstOrDefault() != null ? pv.productVariantImages.FirstOrDefault().ImageName : null 
                })
                .ToListAsync();

            response.Data = productVariants;
            response.Success = true;
            response.Message = "Product variants fetched successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product variants.");
            response.Success = false;
            response.Message = $"An error occurred while fetching product variants: {ex.Message}";
        }

        return response;
    }

}
