
using Ecommerce.Shared.Abstraction;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Entities;

public class Customer: BaseEntity
{
    [Required]
    public  string FirstName { get; set; }
    [Required]
    public  string LastName { get; set; }
    [Required]
    public  string Email { get; set; }
    [Required]
    [StringLength(256)]
    [DataType(DataType.Password)]
    public  string   Password { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = false;
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }









}
