using Ecommerce.Shared.Context;
using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Entities.ProductVariants;
using Ecommerce.Shared.Entities.TrendingProducts;
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
    Task<ServiceResponse<ProductVariantDetailDto>> GetProductVariantDetailByIdAsync(long categoryId);
    Task<ServiceResponse<List<ClusterFeatureDto>>> GetClusterFeaturesAsync(long templateMasterId,long productVariantId);
    Task<ServiceResponse<List<ProductVariantDto>>> GetProductVariantsByBrandAsync(long brandId);
    Task<ServiceResponse<TrendingProduct>> AddTrendingProdutc(TrendingProduct productVariant);
    Task<ServiceResponse<List<TrendingProductDto>>> GetTrendingProductVariantsAsync();
    Task<ServiceResponse<bool>> RemoveTrendingProduct(long id);

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
    public async Task<ServiceResponse<ProductVariantDetailDto>> GetProductVariantDetailByIdAsync(long Id)
    {
        var response = new ServiceResponse<ProductVariantDetailDto>();

        try
        {
            var productVariant = await _context.ProductVariants
                .Include(p => p.Product)
                .Include(p => p.Product.Category)
                .Include(p => p.GeneralColor)
                .Include(p => p.GeneralSize)
                .Include(p => p.ProductVariantFeatureValues)
                .Include(p => p.productVariantImages) 
                .Where(pv => pv.Id == Id)
                .Select(pv => new ProductVariantDetailDto
                {
                    ProductId = pv.ProductId,
                    Id = pv.Id,
                    Name = pv.Name,
                    Category = pv.Product.Category.Name,
                    Color = pv.GeneralColor != null ? pv.GeneralColor.Name : null,
                    Size = pv.GeneralSize != null ? pv.GeneralSize.Name : null,
                    VariantPrice = pv.Price,
                    ProductPrice = pv.Product.Price,
                    Sku = pv.Sku,
                    Description=pv.Description,
                    ProductVariantFeatureValues = pv.ProductVariantFeatureValues,
                    year = pv.ModelYear != null ? pv.ModelYear.Year : 0,
                    TemplateMasterId=pv.Product.TemplateMasterId,
                    DefaultImageUrl = pv.productVariantImages.FirstOrDefault() != null ? pv.productVariantImages.FirstOrDefault().ImageName : null,
                    productVariantImages = pv.productVariantImages.Select(vi => new ProductVariantImages
                    {
                        ImageName = vi.ImageName 
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (productVariant == null)
            {
                response.Success = false;
                response.Message = "Product variant not found.";
            }
            else
            {
                if(productVariant.ProductPrice>0&& productVariant.VariantPrice>0)
                {
                    double discount = productVariant.ProductPrice - productVariant.VariantPrice;
                    int discountPercentage = (int)Math.Round((discount / productVariant.ProductPrice) * 100);
                    productVariant.discountPercentage=discountPercentage;
                }

                response.Data = productVariant;
                response.Success = true;
                response.Message = "Product variant fetched successfully.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product variant.");
            response.Success = false;
            response.Message = $"An error occurred while fetching product variant: {ex.Message}";
        }

        return response;
    }
    public async Task<ServiceResponse<List<ClusterFeatureDto>>> GetClusterFeaturesAsync1(long templateMasterId)
    {
        var response = new ServiceResponse<List<ClusterFeatureDto>>();

        try
        {
            var clusterFeatures = await _context.Set<ClusterFeatureDto>()
                .FromSqlRaw("SELECT c.Name AS Cluster, f.Name AS Feature, c.Id AS ClusterId FROM TemplateClusters tc " +
                            "INNER JOIN Clusters c ON tc.ClusterId = c.Id " +
                            "INNER JOIN TemplateClusterFeatures tf ON tc.Id = tf.TemplateClusterId " +
                            "INNER JOIN Features f ON tf.FeatureId = f.Id " +
                            "WHERE TemplateMasterId = {0}", templateMasterId)
                .ToListAsync();

            var groupedData = clusterFeatures
                .GroupBy(cf => new { cf.Cluster, cf.ClusterId })
                .Select(g => new ClusterFeatureDto
                {
                    Cluster = g.Key.Cluster,
                    ClusterId = g.Key.ClusterId,
                })
                .ToList();

            response.Data = groupedData;
            response.Success = true;
            response.Message = "Cluster features fetched successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching cluster features.");
            response.Success = false;
            response.Message = $"An error occurred while fetching cluster features: {ex.Message}";
        }

        return response;
    }
    public async Task<ServiceResponse<List<ClusterFeatureDto>>> GetClusterFeaturesAsync(long templateMasterId, long productVariantId)
    {
        var response = new ServiceResponse<List<ClusterFeatureDto>>();

        try
        {
            var clusterFeatures = await _context.Set<RawClusterFeatureDto>()
                .FromSqlRaw("SELECT c.Name AS Cluster, f.Name AS Feature, c.Id AS ClusterId, f.Id AS FeatureId, fv.Value FROM TemplateClusters tc " +
                            "INNER JOIN Clusters c ON tc.ClusterId = c.Id " +
                            "INNER JOIN TemplateClusterFeatures tf ON tc.Id = tf.TemplateClusterId " +
                            "INNER JOIN Features f ON tf.FeatureId = f.Id " +
                            "INNER JOIN ProductVariantFeatureValues fv ON tf.Id = fv.TemplateClusterFeatureId " +
                            "WHERE tc.TemplateMasterId = {0} AND fv.ProductVariantId = {1}", templateMasterId, productVariantId)
                .ToListAsync();

            var groupedData = clusterFeatures
                .GroupBy(cf => new { cf.Cluster, cf.ClusterId })
                .Select(g => new ClusterFeatureDto
                {
                    Cluster = g.Key.Cluster,
                    ClusterId = g.Key.ClusterId,
                    Features = g.Select(cf => new FeatureValuePair
                    {
                        Feature = cf.Feature,
                        Value = cf.Value
                    }).ToList()
                })
                .ToList();

            response.Data = groupedData;
            response.Success = true;
            response.Message = "Cluster features fetched successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching cluster features.");
            response.Success = false;
            response.Message = $"An error occurred while fetching cluster features: {ex.Message}";
        }

        return response;
    }
    public async Task<ServiceResponse<List<ProductVariantDto>>> GetProductVariantsByBrandAsync(long brandId)
    {
        var response = new ServiceResponse<List<ProductVariantDto>>();

        try
        {
            var productVariants = await _context.ProductVariants
                .Where(pv => pv.Product.BrandId == brandId)
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
    public async Task<ServiceResponse<TrendingProduct>> AddTrendingProdutc(TrendingProduct product)
    {
        try
        {
            _context.TrendingProducts.Add(product);
            await _context.SaveChangesAsync();
            return new ServiceResponse<TrendingProduct>
            {
                Data = product,
                Success = true,
                Message = "Product  added successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding Trending variant.");
            return new ServiceResponse<TrendingProduct>
            {
                Success = false,
                Message = $"An error occurred while adding the Trending product : {ex.Message}"
            };
        }
    }
    public async Task<ServiceResponse<List<TrendingProductDto>>> GetTrendingProductVariantsAsync()
    {
        var response = new ServiceResponse<List<TrendingProductDto>>();

        try
        {
            var trendingProductVariants = await _context.TrendingProducts
                .Include(tp => tp.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .ThenInclude(p => p.Category)
                .Include(tp => tp.ProductVariant.productVariantImages)
                .Select(tp => new TrendingProductDto
                {
                    ProductId = tp.ProductVariant.ProductId,
                    Id = tp.Id,
                    ProductVariantId= tp.ProductVariant.Id,
                    Name = tp.ProductVariant.Product.Name,
                    Description = tp.ProductVariant.Description,
                    Category = tp.ProductVariant.Product.Category.Name,
                    VariantPrice = tp.ProductVariant.Price,
                    ProductPrice = tp.ProductVariant.Product.Price,
                    Sku = tp.ProductVariant.Sku,
                    DefaultImageUrl = tp.ProductVariant.productVariantImages.FirstOrDefault() != null ? tp.ProductVariant.productVariantImages.FirstOrDefault().ImageName : null
                })
                .ToListAsync();

            // Calculate discount percentage for each product variant
            foreach (var productVariant in trendingProductVariants)
            {
                if (productVariant.ProductPrice > 0 && productVariant.VariantPrice > 0)
                {
                    double discount = productVariant.ProductPrice - productVariant.VariantPrice;
                    int discountPercentage = (int)Math.Round((discount / productVariant.ProductPrice) * 100);
                    productVariant.discountPercentage = discountPercentage;
                }
            }

            response.Data = trendingProductVariants;
            response.Success = true;
            response.Message = "Trending product variants fetched successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching trending product variants.");
            response.Success = false;
            response.Message = $"An error occurred while fetching trending product variants: {ex.Message}";
        }

        return response;
    }

  
    public async Task<ServiceResponse<bool>> RemoveTrendingProduct(long id)
    {
        try
        {
            var tproductVariant = await _context.TrendingProducts.FindAsync(id);
            if (tproductVariant == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Product  not found."
                };
            }

            _context.TrendingProducts.Remove(tproductVariant);
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Product  deleted successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product .");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = $"An error occurred while deleting the product variant: {ex.Message}"
            };
        }
    }

}
