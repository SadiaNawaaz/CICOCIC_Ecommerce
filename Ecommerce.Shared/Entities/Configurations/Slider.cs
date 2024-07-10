

using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Entities.Products;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Shared.Entities.Configurations;

public class Slider: BaseEntity
{

    
    public string? BackgroundImageUrl { get; set; }
    public string? FrontImageUrl { get; set; }
    [Required]
    public string Text { get; set; }
    public int OrderNo { get; set; }
    public bool Active { get; set; }
    public long StartingFrom { get; set; }


    public long ProductId { get; set; }
    public Product Product { get; set; }

    [NotMapped]
    public byte[] BackgroundImageByte { get; set; }

    [NotMapped]
    public byte[] FrontImageByte { get; set; }
}
