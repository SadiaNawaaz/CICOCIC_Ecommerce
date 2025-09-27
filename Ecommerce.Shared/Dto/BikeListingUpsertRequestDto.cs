using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Ecommerce.Shared.Dto;

public class BikeListingUpsertRequestDto
    {
    public string BrandName { get; set; } = default!;
    public string ModelName { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public string? CategoryNameEn { get; set; }
    public string? CategoryNameNl { get; set; }
    public string ColorPrimary { get; set; } = default!;
    public string? ColorSecondary { get; set; }
    public string Size { get; set; } = default!;
    public string Year { get; set; } = default!;
    public double Price { get; set; } = default!;
    public string? Currency { get; set; }
    public string? ShortDescription { get; set; }
    public string ? SerialNumber { get; set; } 
    public string? Productnumber { get; set; }
    public string? Description { get; set; }
    public string FrameNumber { get; set; }


    public string? LockModel { get; set; }
    public string? BrandEngine { get; set; }
    public string? FrameMaterial { get; set; }
    public string? FrameHeight { get; set; }
    public string? GearType { get; set; }
    public string? Gears { get; set; }
    public string? WheelSize { get; set; }
    public string? Audience { get; set; }
    public string? DriveType { get; set; }
    public string? BrakeType { get; set; }
    public string? KeyNumber { get; set; }
    public string? EngineType { get; set; }
    public string? EngineNumber { get; set; }
    public string? DisplaySerialNumber { get; set; }
    public string? BatteryNumber { get; set; }
    public string? BatteryCapacity { get; set; }
    public string? ChargerSerialNumber { get; set; }
    public string? NationalInsuranceNumber { get; set; }
    public string? MileageKm { get; set; }
    public string ? AccelerationType { get; set; }
    public bool PropulsionType { get; set; } = false;

    /*user linking */
    public int ExternalUserId { get; set; } = default!; // CicoBike Users.Id (GUID/string)
    public string Email { get; set; }                    
    public string FirstName { get; set; }
    public string LastName { get; set; }


    public List<ImageIn> Images { get; set; } = new();


    public bool Ok { get; set; }
    }
public sealed class ImageIn
    {
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string DataBase64 { get; set; } = default!;
    }


public sealed class ResolvedRefIds
    {
    public long BrandId { get; set; }
    public long CategoryId { get; set; }
    public long ColorId { get; set; }
    public long SizeId { get; set; }
    public long YearId { get; set; }

    public string BrandSlug { get; set; } = default!;
    public string CategorySlug { get; set; } = default!;
    public string ColorSlug { get; set; } = default!;
    public string SizeSlug { get; set; } = default!;
    public string? YearLabel { get; set; } // e.g., "2020"
    public long ProductId { get; internal set; }
    public long VariantId { get; internal set; }
    public bool UserExists { get; internal set; }
    public bool Published { get; internal set; }
    }
