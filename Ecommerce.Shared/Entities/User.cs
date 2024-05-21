

using System.ComponentModel.DataAnnotations;
namespace Ecommerce.Shared.Entities;

public class User
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }

    [StringLength(100)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

}
