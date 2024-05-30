

using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Clusters;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services.Clusters;
public interface IClusterService
{
    Task<ServiceResponse<List<Cluster>>> GetClustersAsync();
    Task<ServiceResponse<Cluster>> GetClusterByIdAsync(long id);
    Task<ServiceResponse<Cluster>> AddClusterAsync(Cluster cluster);
    Task<ServiceResponse<Cluster>> UpdateClusterAsync(Cluster cluster);
    Task<ServiceResponse<bool>> DeleteClusterAsync(long id);
}

public class ClusterService : IClusterService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ClusterService> _logger;

    public ClusterService(ApplicationDbContext context, ILogger<ClusterService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<Cluster>>> GetClustersAsync()
    {
        try
        {
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
            var cluster = await _context.Clusters.FindAsync(id);
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
            var existingCluster = await _context.Clusters.FindAsync(updatedCluster.Id);

            if (existingCluster == null)
            {
                return new ServiceResponse<Cluster>
                {
                    Success = false,
                    Message = "Cluster not found"
                };
            }

            existingCluster.Name = updatedCluster.Name;
            _context.Clusters.Update(existingCluster);
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
}


