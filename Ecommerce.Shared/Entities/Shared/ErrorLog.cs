

using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Entities.Shared;

public class ErrorLog
{
    [Key]
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string Message { get; set; }

    public string Exception { get; set; }
}
