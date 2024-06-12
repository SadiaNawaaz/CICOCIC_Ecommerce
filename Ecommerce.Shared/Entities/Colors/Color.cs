

using Ecommerce.Shared.Abstraction;

namespace Ecommerce.Shared.Entities.Colors;

public class GeneralColor : BaseEntity
{
    public string Name { get; set; }
    public string HexCode { get; set; } 
}