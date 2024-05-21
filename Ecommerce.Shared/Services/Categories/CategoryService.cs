using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Ecommerce.Shared.Services.Categories;

public interface ICategoryService
{
    Task<List<Category>> GetCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(long id);
    Task AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(long id);
}

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;

    public CategoryService(ApplicationDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        try
        {
            return await _context.Categories.ToListAsync();
        }
        catch (Exception ex)
        {
  
            throw new ServiceException("Error occurred while fetching categories.", ex);
        }
    }

    public async Task<Category> GetCategoryByIdAsync(long id)
    {
        try
        {
            return await _context.Categories.FindAsync(id);
        }
        catch (Exception ex)
        {

            throw new ServiceException("Error occurred while fetching category by ID.", ex);
        }
    }

    public async Task AddCategoryAsync(Category category)
    {
        try
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {

            throw new ServiceException("Error occurred while adding category.", ex);
        }
    }

    public Task UpdateCategoryAsync(Category category)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCategoryAsync(long id)
    {
        throw new NotImplementedException();
    }
}


    public class ServiceException : Exception
    {
        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
