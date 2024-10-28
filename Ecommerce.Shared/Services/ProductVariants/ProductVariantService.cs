using Ecommerce.Shared.Context;
using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Entities.ProductVariants;
using Ecommerce.Shared.Entities.TrendingProducts;
using Ecommerce.Shared.Services.Products;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;

namespace Ecommerce.Shared.Services.ProductVariants;


public interface IProductVariantService
{

    Task<ServiceResponse<ProductVariant>> AddProductVariantAsync(ProductVariant productVariant);
    Task<ServiceResponse<ProductVariant>> UpdateProductVariantAsync(ProductVariant productVariant);
    Task<ServiceResponse<bool>> DeleteProductVariantAsync(long id);
    Task<ServiceResponse<ProductVariant>> GetProductVariantByIdAsync(long id);
    Task<ServiceResponse<List<ProductVariant>>> GetAllProductVariantsAsync(long ProductId,long UserId,bool IsAgent);
    Task<ServiceResponse<List<ProductVariantDto>>> GetProductVariantsByCategoryAsync(long categoryId);
    Task<ServiceResponse<ProductVariantDetailDto>> GetProductVariantDetailByIdAsync(long categoryId);
    Task<ServiceResponse<List<ClusterFeatureDto>>> GetClusterFeaturesAsync(long productVariantId);
    Task<ServiceResponse<List<ProductVariantDto>>> GetProductVariantsByBrandAsync(long brandId);
    Task<ServiceResponse<TrendingProduct>> AddTrendingProdutc(TrendingProduct productVariant);
    Task<ServiceResponse<List<TrendingProductDto>>> GetTrendingProductVariantsAsync();
    Task<ServiceResponse<bool>> RemoveTrendingProduct(long id);
    Task<ServiceResponse<List<ProductVariantDto>>> GetProductVariantsWithinDistanceAsync(string Keyword, long ? CategoryId, string PostalCode, int? Distance);
    Task<ServiceResponse<bool>> SaveVariantObjectMediaAsync(List<VariantObjectMedia> mediaList);
    Task<ServiceResponse<List<CategoryVariantCountDto>>> GetCategoriesWithVariantCountsAsync();
    Task<ServiceResponse<List<ProductVariantDto>>> GetTopTenProductVariantsByCategoryAsync(long categoryId);
    Task<ServiceResponse<List<BrandVariantCountDto>>> GetBrandsWithVariantCountsAsync();
    Task<ServiceResponse<List<ProductVariantDto>>> SearchProductVariantsByKeywordAsync(string keyword);
}

public class ProductVariantService: IProductVariantService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductService> _logger;
    private readonly HttpClient _httpClient; 

    public ProductVariantService(ApplicationDbContext context, ILogger<ProductService> logger, HttpClient httpClient)
    {
        _context = context;
        _logger = logger;
        _httpClient = httpClient;
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
            _logger.LogError(ex, "Error adding product variant."+ex.Message);
            return new ServiceResponse<ProductVariant>
            {
                Success = false,
                InnerException = ex.InnerException.ToString(),
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
                 .Include(pv => pv.ProductVariantMedias)
                .Include(img => img.productVariantImages)
                .Include(img => img.VariantObjectMedias)
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
    public async Task<ServiceResponse<List<ProductVariant>>> GetAllProductVariantsAsync(long ProductId,long UserId, bool IsAgent)
    {
        try
        {
            var productVariantsQuery = _context.ProductVariants
          .Include(pv => pv.GeneralSize)
          .Include(pv => pv.GeneralColor)
          .Include(pv => pv.ModelYear)
           .Where(pv => ProductId == 0 || pv.ProductId == ProductId);
            if (IsAgent)
            {
                productVariantsQuery = productVariantsQuery.Where(pv => pv.CreatedBy == UserId && pv.Sold == 0);
            }

            var productVariants = await productVariantsQuery.ToListAsync();

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
            var category = await _context.Categories.FindAsync(categoryId);

            if (category == null)
                {
                response.Success = false;
                response.Message = "Category not found.";
                return response;
                }

            List<long> categoryIds = new List<long>();

            if (category.Level == 4)
                {

                categoryIds.Add(categoryId);
                }
            else
                {
                categoryIds = await GetAllDescendantCategoryIdsAsync(categoryId);
                }

            // Fetch product variants for the identified categories
            var productVariants = await _context.ProductVariants
                .Where(pv => categoryIds.Contains(pv.Product.CategoryId) && pv.Publish == true && pv.Sold==0)
                .Select(pv => new ProductVariantDto
                    {
                    ProductId = pv.ProductId,
                    Id = pv.Id,
                    Name = pv.Name,
                    Category = pv.Product.Category.Name,
                    Color = pv.GeneralColor != null ? pv.GeneralColor.Name : "No Color",
                    Size = pv.GeneralSize != null ? pv.GeneralSize.Name : "No Size",
                    Year = pv.ModelYear != null ? pv.ModelYear.Year.ToString() : null,
                    Brand = pv.Product.Brand != null ? pv.Product.Brand.Name.ToString() : null,
                    VariantPrice = pv.Price,
                    ProductPrice = pv.Product.Price,
                    Sku = pv.Sku,
                    Thumbnail=pv.Thumbnail,
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
    private async Task<List<long>> GetAllDescendantCategoryIdsAsync(long parentId)
        {
        var allCategories = await _context.Categories.ToListAsync();
        var descendantCategoryIds = new List<long>();

        void AddChildCategories(long categoryId)
            {
            var childCategories = allCategories.Where(c => c.ParentCategoryId == categoryId).ToList();
            foreach (var child in childCategories)
                {
                if (child.Level == 4)
                    {
                    descendantCategoryIds.Add(child.Id);
                    }
                else
                    {
                    AddChildCategories(child.Id); // Recursively add further descendants
                    }
                }
            }

        AddChildCategories(parentId);
        return descendantCategoryIds;
        }


    //public async Task<ServiceResponse<List<ProductVariantDto>>> GetProductVariantsByCategoryAsync(long categoryId)
    //{
    //    var response = new ServiceResponse<List<ProductVariantDto>>();

    //    try
    //    {
    //        var productVariants = await _context.ProductVariants
    //            .Where(pv => pv.Product.CategoryId == categoryId && pv.Publish==true)
    //            .Select(pv => new ProductVariantDto
    //            {
    //                ProductId = pv.ProductId,
    //                Id = pv.Id,
    //                Name = pv.Name,
    //                Category = pv.Product.Category.Name,
    //                Color = pv.GeneralColor != null ? pv.GeneralColor.Name : "No Color",
    //                Size = pv.GeneralSize != null ? pv.GeneralSize.Name : "No Size",
    //                VariantPrice = pv.Price,
    //                ProductPrice = pv.Product.Price,
    //                Sku = pv.Sku,
    //                DefaultImageUrl = pv.productVariantImages.FirstOrDefault() != null ? pv.productVariantImages.FirstOrDefault().ImageName : null 
    //            })
    //            .ToListAsync();

    //        response.Data = productVariants;
    //        response.Success = true;
    //        response.Message = "Product variants fetched successfully.";
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error fetching product variants.");
    //        response.Success = false;
    //        response.Message = $"An error occurred while fetching product variants: {ex.Message}";
    //    }

    //    return response;
    //}
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
                .Include(p => p.Product.Brand)
                .Include(p => p.productVariantImages) 
                .Where(pv => pv.Id == Id)
                .Select(pv => new ProductVariantDetailDto
                {
                    ProductId = pv.ProductId,
                    Id = pv.Id,
                    Name = pv.Name,
                    Category = pv.Product.Category.Name,
                    CategoryId=pv.Product.CategoryId,
                    Color = pv.GeneralColor != null ? pv.GeneralColor.Name : null,
                    Size = pv.GeneralSize != null ? pv.GeneralSize.Name : null,
                    VariantPrice = pv.Price,
                    ProductPrice = pv.Product.Price,
                    Sku = pv.Sku,
                    Description=pv.Description,
                    year = pv.ModelYear != null ? pv.ModelYear.Year : 0,
                    Thumbnail=pv.Thumbnail,
                    DefaultImageUrl = pv.productVariantImages.FirstOrDefault() != null ? pv.productVariantImages.FirstOrDefault().ImageName : null,
                    Brand = pv.Product.Brand != null ? pv.Product.Brand.Name : null,
                    productVariantImages = pv.productVariantImages.Select(vi => new ProductVariantImages
                    {
                        ImageName = vi.ImageName ,
                        Order= vi.Order,
                    }).ToList(),
                    ProductVariantMedia = pv.ProductVariantMedias.Select(vi => new ProductVariantMedia
                    {
                        ImageUrl = vi.ImageUrl,
                        
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

                if (productVariant.ProductVariantMedia == null || !productVariant.ProductVariantMedia.Any())
                    {
                    productVariant.ProductVariantMedia.Add(new ProductVariantMedia
                        {
                        ImageUrl = productVariant.Thumbnail
                        });
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
    public async Task<ServiceResponse<List<ClusterFeatureDto>>> GetClusterFeaturesAsync(long productVariantId)
    {
        var response = new ServiceResponse<List<ClusterFeatureDto>>();

        try
        {
            var clusterFeatures = await _context.Set<RawClusterFeatureDto>()
                .FromSqlRaw("SELECT c.Name AS Cluster, f.Name AS Feature, c.Id AS ClusterId, f.Id AS FeatureId, ISNULL(pcf.Value, '-') AS Value,pc.[Order]" +
                            "FROM ProductCluster pc " +
                            "INNER JOIN Clusters c ON pc.ClusterId = c.Id " +
                            "INNER JOIN ProductClusterFeature pcf ON pc.Id = pcf.ProductClusterId " +
                            "INNER JOIN Features f ON pcf.FeatureId = f.Id " +
                            "WHERE pc.ProductId = {0}", productVariantId)
                .ToListAsync();

            var groupedData = clusterFeatures
                .GroupBy(cf => new { cf.Cluster, cf.ClusterId,cf.Order })
                .Select(g => new ClusterFeatureDto
                {
                    Cluster = g.Key.Cluster,
                    ClusterId = g.Key.ClusterId,
                    Order=g.Key.Order,
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
                .Where(pv => pv.Product.BrandId == brandId&& pv.Sold==0)
                .Select(pv => new ProductVariantDto
                {
                    ProductId = pv.ProductId,
                    Id = pv.Id,
                    Name = pv.Product.Name,
                    Category = pv.Product.Category.Name,
                    Color = pv.GeneralColor != null ? pv.GeneralColor.Name : "No Color",
                    Size = pv.GeneralSize != null ? pv.GeneralSize.Name : "No Size",
                    Year = pv.ModelYear != null ? pv.ModelYear.Year.ToString() : null,
                    VariantPrice = pv.Price,
                    ProductPrice = pv.Product.Price,
                    Sku = pv.Sku,
                    Thumbnail = pv.Thumbnail,
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
    public async Task<ServiceResponse<List<ProductVariantDto>>> GetProductVariantsWithinDistanceAsync(string Keyword, long? CategoryId, string PostalCode, int? Distance)
    {
        var response = new ServiceResponse<List<ProductVariantDto>>();

        try
        {
            var agents = await _context.Users
                .Where(u => u.IsAgent == true && u.PostalCode != null)
                .ToListAsync();

            var agentIdsWithinDistance = new List<long>();

            foreach (var agent in agents)
            {
                var distance = await CalculateDistanceBetweenPostalCodes(PostalCode, agent.PostalCode);
                if (distance <= Distance)
                {
                    agentIdsWithinDistance.Add(agent.Id);
                }
            }
            List<long> categoryIds = new List<long>();
            if (CategoryId.HasValue && CategoryId.Value > 0)
            {
                categoryIds = await GetCategoryAndDescendantsAsync(CategoryId.Value);
            }

            var productVariantsQuery = _context.ProductVariants
                .Include(pv => pv.Product)
                .Include(pv => pv.GeneralSize)
                .Include(pv => pv.GeneralColor)
                .Include(pv => pv.productVariantImages)
                .Where(pv => agentIdsWithinDistance.Contains(pv.CreatedBy.Value));

            if (categoryIds.Any())
            {
                productVariantsQuery = productVariantsQuery.Where(pv => categoryIds.Contains(pv.Product.CategoryId));
            }
            if (!string.IsNullOrEmpty(Keyword))
            {
                productVariantsQuery = productVariantsQuery.Where(pv =>
                    pv.Name.Contains(Keyword) || pv.Product.Name.Contains(Keyword));
            }
            var productVariants = await productVariantsQuery
                .Select(pv => new ProductVariantDto
                {
                    ProductId = pv.ProductId,
                    Id = pv.Id,
                    Name = pv.Name,
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
            _logger.LogError(ex, "Error fetching product variants within distance.");
            response.Success = false;
            response.Message = $"An error occurred while fetching product variants: {ex.Message}";
        }

        return response;
    }
    private async Task<double> CalculateDistanceBetweenPostalCodes(string originPostalCode, string destinationPostalCode)
    {
        string apiKey = ""; 
        string requestUri = $"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={originPostalCode}&destinations={destinationPostalCode}&key={apiKey}";

        HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var jsonResult = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true 
            };
            var distanceMatrixResponse = JsonSerializer.Deserialize<DistanceMatrixResponse>(jsonResult, options);
            if (distanceMatrixResponse.Rows.Count > 0 && distanceMatrixResponse.Rows[0].Elements.Count > 0)
            {
                var distanceInMeters = distanceMatrixResponse.Rows[0].Elements[0].Distance.Value;
                return distanceInMeters / 1000.0; 
            }
        }

        return double.MaxValue; 
    }
 
    private async Task<List<long>> GetCategoryAndDescendantsAsync(long categoryId)
    {
        var allCategories = await _context.Categories.ToListAsync();
        var categoryIds = new List<long> { categoryId };
        AddDescendantCategories(categoryId, allCategories, categoryIds);

        return categoryIds;
    }

    private void AddDescendantCategories(long categoryId, List<Category> allCategories, List<long> categoryIds)
    {
        var childCategories = allCategories.Where(c => c.ParentCategoryId == categoryId).ToList();

        foreach (var child in childCategories)
        {
            if (!categoryIds.Contains(child.Id))
            {
                categoryIds.Add(child.Id);
                AddDescendantCategories(child.Id, allCategories, categoryIds);
            }
        }
    }

    public async Task<ServiceResponse<bool>> SaveVariantObjectMediaAsync(List<VariantObjectMedia> mediaList)
        {
        try
            {
            if (mediaList == null || !mediaList.Any())
                {
                return new ServiceResponse<bool>
                    {
                    Success = false,
                    Message = "No media to save."
                    };
                }

            foreach (var media in mediaList)
                {
                if (media.Id == 0)
                    {
                    _context.VariantObjectMedias.Add(media);
                    }
                else
                    {
                    var existingMedia = await _context.VariantObjectMedias.FindAsync(media.Id);

                    if (existingMedia == null)
                        {
                        return new ServiceResponse<bool>
                            {
                            Success = false,
                            Message = $"Media object with Id {media.Id} not found."
                            };
                        }

                    if (media.IsDeleted)
                        {
                        _context.VariantObjectMedias.Remove(existingMedia);
                        }
                    else
                        {
                        existingMedia.Name = media.Name;
                        existingMedia.FileUrl = media.FileUrl;
                        existingMedia.FilExtension = media.FilExtension;

                        if (media.FileByte != null && media.FileByte.Length > 0)
                            {
                            existingMedia.FileByte = media.FileByte;
                            }
                        }
                    }
                }

            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
                {
                Data = true,
                Success = true,
                Message = "Variant object media saved/updated successfully."
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error saving/updating variant object media.");
            return new ServiceResponse<bool>
                {
                Success = false,
                InnerException=ex.InnerException.ToString(),
                Message = $"An error occurred while saving/updating variant object media"
                };
            }
        }

    public async Task<ServiceResponse<List<CategoryVariantCountDto>>> GetCategoriesWithVariantCountsAsync()
        {
        var response = new ServiceResponse<List<CategoryVariantCountDto>>();

        try
            {
            // Query Level 1 categories and their product variants across multiple levels
            var categoriesWithCounts = await _context.Categories
                .Where(c => c.Level == 1) // Level 1 categories only
                .Select(c => new CategoryVariantCountDto
                    {
                    CategoryId = c.Id,
                    CategoryName = c.Name,

                    // Count product variants from all nested levels (via join)
                    VariantCount = (
                        from level2 in _context.Categories
                        join level3 in _context.Categories on level2.Id equals level3.ParentCategoryId
                        join level4 in _context.Categories on level3.Id equals level4.ParentCategoryId
                        join product in _context.Products on level4.Id equals product.CategoryId
                        join variant in _context.ProductVariants on product.Id equals variant.ProductId
                        where level2.ParentCategoryId == c.Id && variant.Sold==0
                        select variant
                    ).Count()
                    })
              .OrderByDescending(cat => cat.VariantCount) 
            .ToListAsync();

            response.Data = categoriesWithCounts;
            response.Success = true;
            response.Message = "Categories with variant counts fetched successfully.";
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error fetching categories with product variant counts.");
            response.Success = false;
            response.Message = $"An error occurred: {ex.Message}";
            }

        return response;
        }

    public async Task<ServiceResponse<List<ProductVariantDto>>> GetTopTenProductVariantsByCategoryAsync(long categoryId)
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
                    }).Take(10)
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


    public async Task<ServiceResponse<List<BrandVariantCountDto>>> GetBrandsWithVariantCountsAsync()
        {
        var response = new ServiceResponse<List<BrandVariantCountDto>>();

        try
            {
            var brandsWithCounts = await _context.Brands
                .Select(b => new BrandVariantCountDto
                    {
                    BrandId = b.Id,
                    BrandName = b.Name,
                    VariantCount = (
                        from product in _context.Products
                        join variant in _context.ProductVariants on product.Id equals variant.ProductId
                        where product.BrandId == b.Id && variant.Publish == true && variant.Sold==0
                        select variant
                    ).Count()
                    })
                .Where(brand => brand.VariantCount > 0)
                .OrderByDescending(brand => brand.VariantCount)
                .ToListAsync();

            response.Data = brandsWithCounts;
            response.Success = true;
            response.Message = "Brands with variant counts greater than 0 fetched successfully.";
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error fetching brands with product variant counts.");
            response.Success = false;
            response.Message = $"An error occurred: {ex.Message}";
            }

        return response;
        }

    public async Task<ServiceResponse<List<ProductVariantDto>>> SearchProductVariantsByKeywordAsync(string keyword)
        {
        var response = new ServiceResponse<List<ProductVariantDto>>();

        try
            {
            if (string.IsNullOrEmpty(keyword))
                {
                response.Success = false;
                response.Message = "Keyword cannot be empty.";
                return response;
                }

            // Search product variants by keyword in product variant name or product name
            var productVariants = await _context.ProductVariants
                .Include(pv => pv.Product)
                .Include(pv => pv.GeneralSize)
                .Include(pv => pv.GeneralColor)
                .Include(pv => pv.productVariantImages)
               .Where(pv => (pv.Name.Contains(keyword) || pv.Product.Name.Contains(keyword)) && pv.Sold == 0)
                .Select(pv => new ProductVariantDto
                    {
                    ProductId = pv.ProductId,
                    Id = pv.Id,
                    Name = pv.Name,
                    Category = pv.Product.Category.Name,
                    Color = pv.GeneralColor != null ? pv.GeneralColor.Name : "No Color",
                    Size = pv.GeneralSize != null ? pv.GeneralSize.Name : "No Size",
                    Year = pv.ModelYear != null ? pv.ModelYear.Year.ToString() : null,
                    Brand = pv.Product.Brand != null ? pv.Product.Brand.Name.ToString() : null,
                    VariantPrice = pv.Price,
                    ProductPrice = pv.Product.Price,
                    Sku = pv.Sku,
                    Thumbnail = pv.Thumbnail,
                    DefaultImageUrl = pv.productVariantImages.FirstOrDefault() != null ? pv.productVariantImages.FirstOrDefault().ImageName : null
                    })
                .ToListAsync();

            if (!productVariants.Any())
                {
                response.Success = false;
                response.Message = "No product variants found for the given keyword.";
                }
            else
                {
                response.Data = productVariants;
                response.Success = true;
                response.Message = "Product variants fetched successfully.";
                }
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error searching product variants by keyword.");
            response.Success = false;
            response.Message = $"An error occurred while searching product variants: {ex.Message}";
            }

        return response;
        }

    }
