
using Ecommerce.Shared.Abstraction;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Entities;

public class Customer: BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    [StringLength(20)]
    [DataType(DataType.Password)]
    public required string   Password { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = false;
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }









}
