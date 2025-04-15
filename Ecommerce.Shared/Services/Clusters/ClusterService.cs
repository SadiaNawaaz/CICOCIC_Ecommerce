

using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Clusters;
using Ecommerce.Shared.Entities.Products;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.Clusters;
public interface IClusterService
{
    Task<ServiceResponse<List<Cluster>>> GetClustersAsync();
    Task<ServiceResponse<Cluster>> GetClusterByIdAsync(long id);
    Task<ServiceResponse<Cluster>> AddClusterAsync(Cluster cluster);
    Task<ServiceResponse<Cluster>> UpdateClusterAsync(Cluster cluster);
    Task<ServiceResponse<bool>> DeleteClusterAsync(long id);
    Task<ServiceResponse<bool>> ImportFeatureGroupsAsync(List<Cluster> clusters);
    Task<ServiceResponse<List<long>>> GetMissingFeatureGroupsAsync(List<ProductCluster> productClusters);
}

public class ClusterService : IClusterService
{

    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<ClusterService> _logger;

    public ClusterService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<ClusterService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Cluster>>> GetClustersAsync()
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var clusters = await _context.Clusters.ToListAsync();
            return new ServiceResponse<List<Cluster>>
            {
                Data = clusters,
                Success = true,
                Message = "Clusters fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching clusters.");
            return new ServiceResponse<List<Cluster>>
            {
                Success = false,
                Message = "Error occurred while fetching clusters"
            };
        }
    }

    public async Task<ServiceResponse<Cluster>> GetClusterByIdAsync(long id)
        {
        try
            {
            using var _context = _contextFactory.CreateDbContext();

            var cluster = await _context.Clusters
                .Include(c => c.Translations) // Load related translations
                .AsNoTracking() // Ensures EF Core does not track changes (better for read operations)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cluster == null)
                {
                return new ServiceResponse<Cluster>
                    {
                    Success = false,
                    Message = "Cluster not found"
                    };
                }

            return new ServiceResponse<Cluster>
                {
                Data = cluster,
                Success = true,
                Message = "Cluster fetched successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching cluster by ID.");
            return new ServiceResponse<Cluster>
                {
                Success = false,
                Message = "Error occurred while fetching cluster by ID"
                };
            }
        }

    public async Task<ServiceResponse<Cluster>> AddClusterAsync(Cluster cluster)
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            _context.Clusters.Add(cluster);
            await _context.SaveChangesAsync();
            return new ServiceResponse<Cluster>
            {
                Data = cluster,
                Success = true,
                Message = "Cluster added successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding cluster.");
            return new ServiceResponse<Cluster>
            {
                Success = false,
                Message = "Error occurred while adding cluster"
            };
        }
    }

    public async Task<ServiceResponse<Cluster>> UpdateClusterAsync(Cluster updatedCluster)
        {
        try
            {
            using var _context = _contextFactory.CreateDbContext();

            var existingCluster = await _context.Clusters
                .Include(c => c.Translations) // Load existing translations
                .FirstOrDefaultAsync(c => c.Id == updatedCluster.Id);

            if (existingCluster == null)
                {
                return new ServiceResponse<Cluster>
                    {
                    Success = false,
                    Message = "Cluster not found"
                    };
                }

            // ✅ Update Cluster Name
            existingCluster.Name = updatedCluster.Name;

            // ✅ Get existing translations
            var existingTranslations = existingCluster.Translations.ToList();

            // ✅ 1. Remove deleted translations
            var translationsToRemove = existingTranslations
                .Where(oldTranslation => !updatedCluster.Translations
                .Any(t => t.LanguageId == oldTranslation.LanguageId))
                .ToList();

            if (translationsToRemove.Any())
                {
                _context.ClusterTranslations.RemoveRange(translationsToRemove);
                }

            // ✅ 2. Add or update translations
            foreach (var newTranslation in updatedCluster.Translations)
                {
                var existingTranslation = existingTranslations
                    .FirstOrDefault(t => t.LanguageId == newTranslation.LanguageId);

                if (existingTranslation == null)
                    {
                    // Add new translation
                    existingCluster.Translations.Add(new ClusterTranslation
                        {
                        LanguageId = newTranslation.LanguageId,
                        TranslatedName = newTranslation.TranslatedName
                        });
                    }
                else if (existingTranslation.TranslatedName != newTranslation.TranslatedName)
                    {
                    // Update existing translation only if the text has changed
                    existingTranslation.TranslatedName = newTranslation.TranslatedName;
                    }
                }

            await _context.SaveChangesAsync();

            return new ServiceResponse<Cluster>
                {
                Data = existingCluster,
                Success = true,
                Message = "Cluster updated successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while updating cluster.");
            return new ServiceResponse<Cluster>
                {
                Success = false,
                Message = "Error occurred while updating cluster"
                };
            }
        }


    public async Task<ServiceResponse<bool>> DeleteClusterAsync(long id)
    {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var cluster = await _context.Clusters.FindAsync(id);

            if (cluster == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Cluster not found"
                };
            }

            _context.Clusters.Remove(cluster);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Cluster deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting cluster.");
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Error occurred while deleting cluster"
            };
        }
    }



    public async Task<ServiceResponse<bool>> ImportFeatureGroupsAsync(List<Cluster> clusters)
        {
        using var context = _contextFactory.CreateDbContext();
        using var transaction = context.Database.BeginTransaction();

        try
            {
            var hasExplicitIds = clusters.Any(c => c.Id != 0);

            if (hasExplicitIds)
                {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Clusters ON");
                }

            var clusterIds = clusters.Select(c => c.Id).Distinct().ToList();
            var existingClusters = await context.Clusters
                                            .Where(c => clusterIds.Contains(c.Id))
                                            .ToListAsync();

            var newClusters = clusters.Where(c => !existingClusters.Any(ec => ec.Id == c.Id)).ToList();
            var updatedClusters = clusters.Where(c => existingClusters.Any(ec => ec.Id == c.Id)).ToList();

            if (newClusters.Any())
                {
                context.Clusters.AddRange(newClusters);
                }

            foreach (var updatedCluster in updatedClusters)
                {
                var existingCluster = existingClusters.FirstOrDefault(ec => ec.Id == updatedCluster.Id);
                if (existingCluster != null)
                    {
                    existingCluster.Name = updatedCluster.Name;
                    context.Clusters.Update(existingCluster);
                    }
                }

            await context.SaveChangesAsync();

            if (hasExplicitIds)
                {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Clusters OFF");
                }

            transaction.Commit();

            return new ServiceResponse<bool>
                {
                Data = true,
                Success = true,
                Message = "Clusters imported successfully"
                };
            }
        catch (Exception ex)
            {
            transaction.Rollback();
            _logger.LogError(ex, "Error occurred while importing clusters.");
            return new ServiceResponse<bool>
                {
                Data = false,
                Success = false,
                Message = "Error occurred while importing clusters"
                };
            }
        }



    public async Task<ServiceResponse<List<long>>> GetMissingFeatureGroupsAsync(List<ProductCluster> productClusters)
        {
        using var _context = _contextFactory.CreateDbContext();
        var existingIds = await _context.Clusters
            .Select(c => c.Id)
            .ToListAsync();

        var requestedIds = productClusters.Select(c => c.ClusterId).Distinct();

        var missing = requestedIds.Except(existingIds).ToList();

        return new ServiceResponse<List<long>> { Data = missing, Success = true };
        }



    }


