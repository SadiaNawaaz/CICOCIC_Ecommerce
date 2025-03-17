using AspNetCore.Reporting;
using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Services.ProductVariants;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;
using System.Data;
using System.Net;
namespace Ecommerce.Admin.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
    {

    private readonly ILogger<ReportController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IProductVariantService _variantService;

    public ReportController(ILogger<ReportController> logger, IWebHostEnvironment webHostEnvironment, IProductVariantService productVariantService)
        {
        _logger = logger;
        this._webHostEnvironment = webHostEnvironment;
        _variantService = productVariantService;
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        }

    [HttpGet("Print")]

    public async Task<IActionResult> Print(long VariantId)
        {
   try
            {
            string printedDate = $"Printed on {DateTime.Now:dd-MM-yyyy HH:mm:ss}";
            var Data = await _variantService.GetProductVariantForReport(VariantId);
            var FeaturesGroup = await _variantService.GetClusterFeaturesAsync(Data.Data.ProductId, "en");
            var imagebytes = GetImageFromUrl(Data.Data.Thumbnailurl);
            Data.Data.Thumbnail = imagebytes;
            Data.Data.PrintedOn = printedDate;
            DataTable dtVariant = ConvertToProductVariantTable(Data.Data);
            DataTable dtFeatures = ConvertToFeatureValuesTable(FeaturesGroup.Data);

            // Load RDLC report
           // var path = "C:\\Users\\ZBOOK\\source\\repos\\rdlcProject\\ValidationReport.rdlc";
           var path=$"{this._webHostEnvironment.WebRootPath}\\Reports\\ValidationReport.rdlc";
            //$"{this._webHostEnvironment.WebRootPath}\\Reports\\ValidationReport.rdlc";


            string mimetype = "";
            int extension = 1;
          
      

            LocalReport localReport = new LocalReport(path);

            localReport.AddDataSource("DataSet1", dtVariant);
            localReport.AddDataSource("DataSet2", dtFeatures);
            /// localReport.AddDataSource("FeatureValueDataset", dtFeatures);
            var result = localReport.Execute(RenderType.Pdf, extension, null, mimetype);
            return File(result.MainStream, "application/pdf");
            }
        catch(Exception ex)
            {
            return StatusCode(500, new { message = "An error occurred while generating the report.", error = ex.Message });
            }
        
     
        }

    private ProductVariantDetailDto1 GetProductVariantById(long productId)
        {

        byte[] imageBytes = GetImageFromUrl("https://cicocic.s3.eu-central-1.amazonaws.com/product_CIC_48099/CIC_48099_1.jpg");
        return new ProductVariantDetailDto1
            {
            ProductId = productId,
            Name = "T-Shirt",
            Category = "Clothing",
            Brand = "Nike",
            Color = "Red",
            Size = "M",
            year = 2023,
            ProductPrice = 20.0,
            VariantPrice = 18.0,
            Sku = "TSHIRT-NIKE-RED-M",
            SSN = "123456789",
            ShortDescription = "A comfortable cotton T-shirt in red color.",
            LongDescription = "A comfortable cotton T-shirt in red color sdasdasdasddddddddddddddddddddddddddddddddddddddddddd.",
            ProductCode = "NIKE-TSH-RED-M",
            EanNumber = "8901234567890",
            VariantType = "IMEI number",
            Value = "Medium",
            Thumbnail = imageBytes,
            ProductVariantFeatureValues = new List<ProductVariantFeatureValue1>
        {
            new ProductVariantFeatureValue1 {GroupId=1, GroupName = "Material", FeatureName = "Fabric", FeatureValue = "Cotton" },
            new ProductVariantFeatureValue1 { GroupId=1, GroupName = "Material", FeatureName = "Durability", FeatureValue = "High" },
            new ProductVariantFeatureValue1 { GroupId=2, GroupName = "Comfort", FeatureName = "Fit", FeatureValue = "Regular" },
            new ProductVariantFeatureValue1{ GroupId=2, GroupName = "Comfort", FeatureName = "Size", FeatureValue = "Small" },
            new ProductVariantFeatureValue1 { GroupId=3, GroupName = "Branding", FeatureName = "Logo", FeatureValue = "Printed" }
        }
            };
        }



    public static DataTable ConvertToProductVariantTable(ObjectDto variant)
        {
        DataTable dt = new DataTable();

    


        // Add columns dynamically based on the DTO properties
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Category", typeof(string));
        dt.Columns.Add("Brand", typeof(string));
        dt.Columns.Add("Color", typeof(string));
        dt.Columns.Add("Size", typeof(string));
        dt.Columns.Add("Year", typeof(int));
        dt.Columns.Add("ProductPrice", typeof(double));
        dt.Columns.Add("VariantPrice", typeof(double));
        dt.Columns.Add("Sku", typeof(string));
        dt.Columns.Add("SSN", typeof(string));
        dt.Columns.Add("ShortDescription", typeof(string));
        dt.Columns.Add("LongDescription", typeof(string));
        dt.Columns.Add("ProductCode", typeof(string));
        dt.Columns.Add("EanNumber", typeof(string));
        dt.Columns.Add("VariantType", typeof(string));
        dt.Columns.Add("Value", typeof(string));
        dt.Columns.Add("Thumbnail", typeof(byte[]));
        dt.Columns.Add("PrintedOn", typeof(string));

        // Add row with data
        dt.Rows.Add(
            variant.Name,
            variant.Category,
            variant.Brand,
            variant.Color,
            variant.Size,
            variant.year,
            variant.ProductPrice,
            variant.VariantPrice,
            variant.Sku,
            variant.SSN,
            variant.ShortDescription,
              variant.LongDescription,
            variant.ProductCode,
            variant.EanNumber,
            variant.VariantType,
            variant.Value,
            variant.Thumbnail,
            variant.PrintedOn
        );

        return dt;
        }



    public static DataTable ConvertToFeatureValuesTable(List<ClusterFeatureDto> Features)
        {
        DataTable dt = new DataTable();

        dt.Columns.Add("GroupName", typeof(string));
        dt.Columns.Add("FeatureName", typeof(string));
        dt.Columns.Add("FeatureValue", typeof(string));
        dt.Columns.Add("GroupId", typeof(long));

        foreach (var group in Features)
            {
            foreach (var feature in group.Features)
                {
                dt.Rows.Add(group.Cluster, feature.Feature, feature.Value, group.ClusterId);
                }
           
            }
        return dt;
        }
    public byte[] GetImageFromUrl(string imageUrl)
        {

        try
            {
            using (WebClient client = new WebClient())
                {
                return client.DownloadData(imageUrl); // Fetch image as byte array
                }
            }
        catch(Exception ex)
            {
            return null;
            }
      
        }

    }

public class ProductVariantDetailDto1
    {

    public long ProductId { get; set; } 
    public string Name { get; set; }
    public string Category { get; set; }
    public string? Brand { get; set; }
    public string? Color { get; set; }
    public string? Size { get; set; }
    public int year { get; set; }
    public double ProductPrice { get; set; }
    public double VariantPrice { get; set; }
    public string? Sku { get; set; }
    public string? SSN { get; set; }
    public string? ShortDescription { get; set; }
    public string? LongDescription { get; set; }
    public string? ProductCode { get; set; }
    public string? EanNumber { get; set; }
    public string VariantType { get; set; }
    public string? Value { get; set; }
    public byte[] Thumbnail { get; set; }
    public List<ProductVariantFeatureValue1> ProductVariantFeatureValues { get; set; } = new List<ProductVariantFeatureValue1>();

    }
public class ProductVariantFeatureValue1
    {
    public long GroupId { get; set; }
    public string GroupName { get; set; }
    public string FeatureName { get; set; }
    public string? FeatureValue { get; set; }
    }


