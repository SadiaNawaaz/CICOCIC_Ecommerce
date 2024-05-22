
using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities.Shared;

namespace Ecommerce.Shared.Services.Shared;
public class ServiceException : Exception
{
   private readonly ApplicationDbContext _context;

    public ServiceException(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogErrorToDatabaseAsync(string message, Exception exception)
    {
        var errorLog = new ErrorLog
        {
            Timestamp = DateTime.UtcNow,
            Message = message,
            Exception = exception.ToString()
        };

        _context.ErrorLogs.Add(errorLog);
        await _context.SaveChangesAsync();
    }
}

