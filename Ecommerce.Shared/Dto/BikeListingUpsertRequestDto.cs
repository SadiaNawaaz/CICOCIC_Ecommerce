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
    public string ColorName { get; set; } = default!;
    public string Size { get; set; } = default!;
    public string Year { get; set; } = default!;
    public double Price { get; set; } = default!;
    public string? Currency { get; set; }
    public string? ShortDescription { get; set; }
    public string ? SerialNumber { get; set; } 
    public string? Productnumber { get; set; }
    public string? Description { get; set; }
    public string FrameNumber { get; set; }




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
    }
