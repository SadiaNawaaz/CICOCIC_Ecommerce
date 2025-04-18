﻿using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Entities.Catalogs;
using Ecommerce.Shared.Entities.CategoryFeatures;
using Ecommerce.Shared.Entities.Clusters;
using Ecommerce.Shared.Entities.Features;
using Ecommerce.Shared.Entities.Products;
using Ecommerce.Shared.Services.Brands;
using Ecommerce.Shared.Services.Catalogs;
using Ecommerce.Shared.Services.Categories;
using Ecommerce.Shared.Services.CategoryFeatures;
using Ecommerce.Shared.Services.Clusters;
using Ecommerce.Shared.Services.Features;
using Ecommerce.Shared.Services.Products;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Ecommerce.Shared.Services;


public interface IDataDownloads
    {

    Task DownloadAndSaveCategories();
    Task DownloadAndSaveBrands();
    Task DownloadAndSaveFeatureGroups();
    Task DownloadAndSaveFeatures();
     Task<bool> DownloadAndSaveAllProducts(CatalogDto catalogDto);
    Task DownloadAndSaveCategoryFeatures();
    Task DownloadAndSaveAllCatalogs();


    }


public class DataDownloads: IDataDownloads
    {
    private readonly string _username = "sadial217428@gmail.com";
    private readonly string _password = "sadial217428@gmail.com";
    private readonly string _categoriesUrl = "https://data.Icecat.biz/export/freexml/refs/CategoriesList.xml.gz";
    private readonly string _brandsUrl = "https://data.Icecat.biz/export/freexml/refs/SuppliersList.xml.gz"; // URL for brands
    private readonly string _featuresUrl = "https://data.Icecat.biz/export/freexml/refs/FeaturesList.xml.gz";
    private readonly string _featureGroupsUrl = "https://data.Icecat.biz/export/freexml/refs/FeatureGroupsList.xml.gz";
    private readonly string _productsUrl = "https://live.icecat.biz/api?UserName={0}&Language=en&icecat_id={1}";
    private readonly string _categoryFeaturesUrl = "https://data.icecat.biz/export/freexml.int/refs/CategoryFeaturesList.xml.gz";
                                                    


    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;
    private readonly IClusterService _featureGroupService;

    private readonly IFeatureService _featureService;
    private readonly ICategoryFeatureService _categoryFeatureService;

    private readonly ICatalogService _catalogService;
    private readonly IProductService _productService;
    public DataDownloads(ICategoryService categoryService, IBrandService brandService, IClusterService featureGroupService,IFeatureService featureService, ICategoryFeatureService categoryFeatureService, ICatalogService catalogService,IProductService productService)
        {
        _categoryService = categoryService;
        _brandService = brandService;
        _featureGroupService = featureGroupService;
        _featureService= featureService;
        _categoryFeatureService= categoryFeatureService;
        _catalogService = catalogService;
        _productService= productService;

        }



    #region Category

    public async Task DownloadAndSaveCategories()
        {
        var categories = await DownloadCategories();
        await SaveCategoriesToDatabase(categories);
        }

    private async Task<List<Category>> DownloadCategories()
        {

        try
            {
            using WebClient client = new WebClient();
            client.Credentials = new NetworkCredential(_username, _password);

            byte[] data = await client.DownloadDataTaskAsync(_categoriesUrl);
            using var decompressedStream = new MemoryStream();
            using var compressedStream = new MemoryStream(data);
            using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            gzipStream.CopyTo(decompressedStream);
            decompressedStream.Seek(0, SeekOrigin.Begin);

            var xmlContent = Encoding.UTF8.GetString(decompressedStream.ToArray());
            XDocument doc = XDocument.Parse(xmlContent);

            List<Category> categories = doc.Descendants("Category")
                .Select(cat => new Category
                    {
                    Id = (int)cat.Attribute("ID"),
                    Name = cat.Elements("Name")
                              .FirstOrDefault(n => (int)n.Attribute("langid") == 1)?.Attribute("Value")?.Value,
                    ParentCategoryId = cat.Element("ParentCategory") != null ? (int?)cat.Element("ParentCategory").Attribute("ID") : null,
                    CreatedDate=DateTime.Now,
                    Translations = cat.Elements("Name")
                    .Where(n => new[] { 1, 2, 4 }.Contains((int)n.Attribute("langid"))) // English, Dutch, German
                    .Select(n => new CategoryTranslation
                        {
                        LanguageId = (int)n.Attribute("langid"),
                        TranslatedName = n.Attribute("Value")?.Value ?? string.Empty
                        })
                    .ToList()


                    }).ToList();

            return categories;
            }
        catch (Exception ex)
            {
            throw;
            }
            }


    private async Task SaveCategoriesToDatabase(List<Category> categories)
        {
        var response = await _categoryService.ImportCategoriesAsync(categories);
        }
    #endregion


    #region Brands 


    public async Task DownloadAndSaveBrands()
        {
        var brands = await DownloadBrands();
        await SaveBrandsToDatabase(brands);
        }
    private async Task<List<Brand>> DownloadBrands()
        {
        using WebClient client = new WebClient();
        client.Credentials = new NetworkCredential(_username, _password);

        string brandsUrl = _brandsUrl;
        byte[] data = await client.DownloadDataTaskAsync(brandsUrl);
        using var decompressedStream = new MemoryStream();
        using var compressedStream = new MemoryStream(data);
        using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
        gzipStream.CopyTo(decompressedStream);
        decompressedStream.Seek(0, SeekOrigin.Begin);

        var xmlContent = Encoding.UTF8.GetString(decompressedStream.ToArray());
        XDocument doc = XDocument.Parse(xmlContent);

        List<Brand> brands = doc.Descendants("Supplier")
            .Select(sup => new Brand
                {
                Id = (int)sup.Attribute("ID"),
                Name = sup.Attribute("Name")?.Value, 
                ThumbnailFileUrl = sup.Attribute("LogoPic").Value,
                ImageUrl = sup.Attribute("LogoHighPic").Value,
                }).ToList();

        return brands;
        }


    private async Task SaveBrandsToDatabase(List<Brand> brands)
        {
        var response = await _brandService.ImportBrandsAsync(brands);
        if (!response.Success)
            {
            // Handle errors
            Console.WriteLine("Failed to import brands: " + response.Message);
            }
        }




    #endregion


    #region FeatureGroup

    public async Task DownloadAndSaveFeatureGroups()
        {
        var featureGroups = await DownloadFeatureGroups();
        await SaveFeatureGroupsToDatabase(featureGroups);
        }

    private async Task<List<Cluster>> DownloadFeatureGroups()
        {
      
        WebClient client = new WebClient();
        client.Credentials = new NetworkCredential(_username, _password);
        byte[] data = await client.DownloadDataTaskAsync(_featureGroupsUrl);

        using var compressedStream = new MemoryStream(data);
        using var decompressedStream = new MemoryStream();
        using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
        gzipStream.CopyTo(decompressedStream);
        decompressedStream.Seek(0, SeekOrigin.Begin);

        var xmlContent = Encoding.UTF8.GetString(decompressedStream.ToArray());
        XDocument doc = XDocument.Parse(xmlContent);

        List<Cluster> featureGroups = doc.Descendants("FeatureGroup")
            .Select(fg => new Cluster
                {
                Id = (int)fg.Attribute("ID"),
                Name = fg.Elements("Name")
                     .FirstOrDefault(n => (int)n.Attribute("langid") == 1)?.Attribute("Value")?.Value ?? "Default Name",

                Translations = fg.Elements("Name")
                    .Where(n => new[] { 1, 2, 4 }.Contains((int)n.Attribute("langid"))) // English, Dutch, German
                    .Select(n => new ClusterTranslation
                        {
                        LanguageId = (int)n.Attribute("langid"),
                        TranslatedName = n.Attribute("Value")?.Value ?? string.Empty
                        })
                    .ToList()

                }).ToList();

        return featureGroups;
        }

    private async Task SaveFeatureGroupsToDatabase(List<Cluster> featureGroups)
        {
        var response = await _featureGroupService.ImportFeatureGroupsAsync(featureGroups.Where(a=>a.Id>0).ToList());
  
        }

    #endregion

    #region Features



    public async Task DownloadAndSaveFeatures()
        {
        var features = await DownloadFeatures();
        await SaveFeaturesToDatabase(features);
        }
    private async Task<List<Feature>> DownloadFeatures()
        {
        WebClient client = new WebClient();
        client.Credentials = new NetworkCredential(_username, _password);
        byte[] data = await client.DownloadDataTaskAsync(_featuresUrl);

        using var compressedStream = new MemoryStream(data);
        using var decompressedStream = new MemoryStream();
        using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
        gzipStream.CopyTo(decompressedStream);
        decompressedStream.Seek(0, SeekOrigin.Begin);

        var xmlContent = Encoding.UTF8.GetString(decompressedStream.ToArray());
        XDocument doc = XDocument.Parse(xmlContent);

        List<Feature> features = doc.Descendants("Feature")
       .Select(f => new Feature
           {
           Id = (int)f.Attribute("ID"),
           Name = f.Elements("Names").Elements("Name")
                .FirstOrDefault(n => (int)n.Attribute("langid") == 1)?.Value ?? "",

           ClusterId =1,
           Translations = f.Element("Names")?.Elements("Name")
                    .Where(n => new[] { 1, 2, 4 }.Contains((int)n.Attribute("langid"))) // English, Dutch, German
                    .Select(n => new FeatureTranslation
                        {
                        LanguageId = (int)n.Attribute("langid"),
                        TranslatedName = n.Value ?? string.Empty
                        })
                    .ToList() ?? new List<FeatureTranslation>(),
           }).ToList();

        return features;
        }


    private async Task SaveFeaturesToDatabase(List<Feature> featureGroups)
        {

        var response = await _featureService.ImportFeaturesAsync(featureGroups.Where(a => a.Id > 0).ToList());
        }

    #endregion


    #region Products


    private async Task<Product> FetchProductDetails(CatalogDto catalog)
        {
        try
            {
         

            var languages = new Dictionary<string, int>
        {
            { "en", 1 }, // English (Default)
            { "nl", 2 }, // Dutch
            { "de", 4 }  // German
        };

            Product product = null;

            foreach (var lang in languages)
                {
                string apiUrl = $"https://live.icecat.biz/api?UserName=openIcecat-live&Language={lang.Key}&icecat_id={catalog.IntegratedId}";
                using HttpClient client = new HttpClient();
                string jsonResponse = await client.GetStringAsync(apiUrl);

                JObject response = JObject.Parse(jsonResponse);
                if (response == null || response["data"] == null || !response["data"].HasValues)
                    continue;

                // Extract necessary product details
                JObject generalInfo = (JObject)response["data"]["GeneralInfo"];
                JObject image = (JObject)response["data"]["Image"];
                JArray gallery = (JArray)response["data"]["Gallery"];
                JArray featuresGroups = (JArray)response["data"]["FeaturesGroups"];
                JArray multimedia = (JArray)response["data"]["Multimedia"];

                if (generalInfo == null || image == null)
                    continue;

                string productName = generalInfo["ProductName"]?.ToString();
                string description = generalInfo["SummaryDescription"]?["LongSummaryDescription"]?.ToString();
                string shortDescription = generalInfo["SummaryDescription"]?["ShortSummaryDescription"]?.ToString();
                long? brandId = (long?)generalInfo["BrandID"];
                long? categoryId = generalInfo["Category"]?["CategoryID"]?.ToObject<long>();
                string thumbnail = image["HighPic"]?.ToString();
                string productCode = generalInfo["BrandPartCode"]?.ToString() ?? "";
                string productTransName = generalInfo["TitleInfo"]?["GeneratedLocalTitle"]?["Value"]?.ToString();
                string slug = GenerateSlug(shortDescription);

                if (productName == null || brandId == null || categoryId == null || thumbnail == null)
                    continue;

                if (lang.Key == "en") // Save English as the default product
                    {
                    double price = await FetchProductPriceByUPC(catalog.EanNumber);
                    product = new Product
                        {
                        CatalogId = catalog.Id,
                        Name = productName,
                        Price = price,
                        Code = productCode,
                        Description = description,
                        ShortDescription = shortDescription,
                        BrandId = brandId.Value,
                        CategoryId = categoryId.Value,
                        Thumbnail = thumbnail,
                        EanNumber = catalog.EanNumber,
                        Slug = slug,
                        ProductClusters = new List<ProductCluster>(),
                        ProductImages = new List<ProductImages>(),
                        ProductMedias = new List<ProductMedia>(),
                        Translations = new List<ProductTranslation>()
                        };
                    // Handle Gallery Images
                    if (gallery != null)
                        {
                        foreach (JObject item in gallery)
                            {

                            var imageToUse = new[] { item["Pic500x500"], item["HighPic"], item["LowPic"], item["ThumbPic"] }.Select(i => i?.ToString()).FirstOrDefault(s => !string.IsNullOrEmpty(s));

                            if (imageToUse != null)
                                {
                                product.ProductImages.Add(new ProductImages { ImageName = imageToUse });
                                }


                          /*  product.ProductImages.Add(new ProductImages
                                {
                                ImageName = item["Pic500x500"]?.ToString()
                                });*/
                            }
                        }

                    // Handle Multimedia
                    if (multimedia != null)
                        {
                        foreach (JObject item in multimedia)
                            {
                            product.ProductMedias.Add(new ProductMedia
                                {
                                MediaUrl = item["URL"]?.ToString(),
                                ContentType = item["ContentType"]?.ToString()
                                });
                            }
                        }

                    // Handle FeaturesGroups
                    if (featuresGroups != null)
                        {
                        foreach (JObject fg in featuresGroups)
                            {
                            ProductCluster cluster = new ProductCluster
                                {
                                ClusterId = (long)fg["FeatureGroup"]["ID"],
                                ProductClusterFeatures = new List<ProductClusterFeature>()
                                };

                            JArray features = (JArray)fg["Features"];
                            foreach (JObject f in features)
                                {
                                cluster.ProductClusterFeatures.Add(new ProductClusterFeature
                                    {
                                    FeatureId = (long)f["Feature"]["ID"],
                                    Value = f["PresentationValue"]?.ToString()
                                    });
                                }

                            product.ProductClusters.Add(cluster);
                            }
                        }

                    }

                // Add Translation for Each Language (Including English)
                product.Translations.Add(new ProductTranslation
                    {
                    Product = product,
                    LanguageId = lang.Value,
                    TranslatedName = productTransName ?? productName, // Fallback to default if null
                    TranslatedDescription = description,
                    TranslatedShortDescription = shortDescription
                    });

              
                }

            return product;
            }
        catch (Exception ex)
            {
            throw;
            }
        }


    private async Task<Product> FetchProductDetails1(CatalogDto catalog)
        {
        try
            {
            string apiUrl = $"https://live.icecat.biz/api?UserName=openIcecat-live&Language=en&icecat_id={catalog.IntegratedId}";
            using HttpClient client = new HttpClient();
            string jsonResponse = await client.GetStringAsync(apiUrl);

            JObject response = JObject.Parse(jsonResponse);
            if (response == null || response["data"] == null || !response["data"].HasValues)
                {
                return null;
                }

            // Extract GeneralInfo and Image safely
            JObject generalInfo = (JObject)response["data"]["GeneralInfo"];
            JObject image = (JObject)response["data"]["Image"];
            JArray Gallery = (JArray)response["data"]["Gallery"];
            JArray featuresGroups = (JArray)response["data"]["FeaturesGroups"];
            JArray Multimedia = (JArray)response["data"]["Multimedia"];
            if (generalInfo == null || image == null)
                {
                return null; 
                }

            // Safe extraction of properties BrandPartCode
            string productName = (string)generalInfo["ProductName"];
            string description = generalInfo["Description"]?["LongDesc"]?.ToString();
            string shortDescription = generalInfo["SummaryDescription"]?["ShortSummaryDescription"]?.ToString();
            long? brandId = (long?)generalInfo["BrandID"];
            long? categoryId = generalInfo["Category"]?["CategoryID"]?.ToObject<long>();
            string thumbnail = (string)image["HighPic"];
            string productCode = generalInfo["BrandPartCode"]?.ToString() ?? "";
            string slug = GenerateSlug(productName);

            if (productName == null || brandId == null || categoryId == null || thumbnail == null)
                {
                return null; 
                }
            double price = await FetchProductPriceByUPC(catalog.EanNumber);
            Product product = new Product
                {
                CatalogId= catalog.Id,
                Name = productName,
                Price= price,
                Code = productCode,
                Description = description,
                ShortDescription = shortDescription,
                BrandId = brandId.Value,              
                CategoryId = categoryId.Value,
                Thumbnail = thumbnail,
                EanNumber= catalog.EanNumber,
                Slug= slug,
                ProductClusters = new List<ProductCluster>()
                };

            // Handle Multimedia
            if (Gallery != null)
                {
                foreach (JObject item in Gallery)
                    {
             
                    ProductImages imagelink = new ProductImages();
                    imagelink.ImageName = item["Pic500x500"].ToString();
                    product.ProductImages.Add(imagelink);

                    }
                }
            //ProductMedias

            if (Multimedia != null)
                {
                foreach (JObject item in Multimedia)
                    {

                    ProductMedia catalogMedia = new ProductMedia();
                    catalogMedia.MediaUrl = item["URL"].ToString();
                    catalogMedia.ContentType = item["ContentType"].ToString();
                    product.ProductMedias.Add(catalogMedia);

                    }
                }




            // Handle FeaturesGroups
            if (featuresGroups != null)
                {
                foreach (JObject fg in featuresGroups)
                    {
                    ProductCluster cluster = new ProductCluster
                        {
                        ClusterId = (long)fg["FeatureGroup"]["ID"],
                        ProductClusterFeatures = new List<ProductClusterFeature>()
                        };

                    JArray features = (JArray)fg["Features"];
                    foreach (JObject f in features)
                        {
                        ProductClusterFeature feature = new ProductClusterFeature
                            {
                            FeatureId = (long)f["Feature"]["ID"],
                            Value = (string)f["PresentationValue"]
                            };
                        cluster.ProductClusterFeatures.Add(feature);
                        }

                    product.ProductClusters.Add(cluster);
                    }
                }

            return product;
            }
        catch (Exception ex)
            {
          
            throw; 
            }
        }

    public async Task<bool> DownloadAndSaveAllProducts(CatalogDto catalogDto)
        {
        try
            {
            var productDetails = await FetchProductDetails(catalogDto);


            //Check if FeatureGroup not found
            var clusterResponse = await _featureGroupService.GetMissingFeatureGroupsAsync(productDetails.ProductClusters);
            if (clusterResponse.Data.Count > 0)
                {
                var downloadedGroups = await DownloadFeatureGroupsAsync(clusterResponse.Data);
                await SaveFeatureGroupsToDatabase(downloadedGroups);
                }

            //Check Features if not found.
            var featureResponse=await _featureService.GetMissingFeaturesAsync(productDetails.ProductClusters);
            if(featureResponse.Data.Count>0)
                {
                var downloadedFeatures = await DownloadFeaturesAsync(featureResponse.Data);
                await SaveFeaturesToDatabase(downloadedFeatures);
                }
            var serviceResponse= await _productService.AddProductAsync(productDetails);
            if(serviceResponse.Success)
                {
                return true;
                }
            else
                {
                return false;
                }
       
            }
        catch (Exception)
            {
           
            return false; 
            }
        }

    private async Task<double> FetchProductPriceByUPC(string upc)
        {
        try
            {
            string apiUrl = $"https://api.upcitemdb.com/prod/trial/lookup?upc={upc}";
            using HttpClient client = new HttpClient();
            string jsonResponse = await client.GetStringAsync(apiUrl);

            JObject response = JObject.Parse(jsonResponse);
            if (response?["items"] != null && response["items"].Any())
                {
                // Assuming price is in the first item's "lowest_recorded_price"
                double price = (double)response["items"][0]["lowest_recorded_price"];
                return price;
                }
            }
        catch (Exception ex)
            {
            // Log the error or handle it appropriately
            Console.WriteLine($"Error fetching price: {ex.Message}");
            }
        return 0;
        }




    private string GenerateSlug(string value)
        {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        value=ExtractBeforeFirstComma(value);
        // Convert to lower case and remove all accents
        var normalizedString = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
            {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                stringBuilder.Append(c);
                }
            }

        // Remove invalid characters
        string slug = Regex.Replace(stringBuilder.ToString(), @"[^a-z0-9\s-]", "");

        // Replace white spaces with hyphen, then trim and remove duplicate hyphens
        slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
        slug = Regex.Replace(slug, @"\-{2,}", "-");

        return slug;
        }

    public string ExtractBeforeFirstComma(string input)
        {
        int index = input.IndexOf(',');
        return index != -1 ? input.Substring(0, index) : input;
        }

    #endregion


    #region  categoryFeatures


    public async Task DownloadAndSaveCategoryFeatures()
        {
        var categoryFeatures = await DownloadCategoryFeatures();
        await SaveCategoryFeaturesToDatabase(categoryFeatures);
        }

    //      string localFilePath = @"F:\buurter\CategoryFeaturesList (1).xml";



    public async Task<List<CategoryFeature>> DownloadCategoryFeatures()
        {
        string localFilePath = @"F:\buurter\CategoryFeaturesList (1).xml";
        List<CategoryFeature> categoryFeatures = new List<CategoryFeature>();
        try
            {
            using (FileStream fileStream = File.OpenRead(localFilePath))
            using (XmlReader reader = XmlReader.Create(fileStream, new XmlReaderSettings { Async = true, DtdProcessing = DtdProcessing.Parse }))
                {
                int categoryId = 0;  // To store current category ID
                int categoryFeatureGroupId = 0;  // To store current CategoryFeatureGroup ID
                int featureGroupId = 0;  // To store current FeatureGroup ID
                int featureId = 0;  // To store the current Feature ID
                string featureName = null;  // To temporarily hold the feature name if the language ID is 1
               

                while (await reader.ReadAsync())
                    {
                    switch (reader.NodeType)
                        {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                                {
                                case "Category":
                                    if (int.TryParse(reader.GetAttribute("ID"), out var catId))
                                        {
                                        categoryId = catId;  // Capture the category ID
                                        if (catId != 788)
                                            {
                                            categoryId = 0;  // Reset categoryId if it's not the one we're interested in
                                            }
                                        }
                                    break;
                                case "CategoryFeatureGroup":
                                    if (categoryId == 788 && int.TryParse(reader.GetAttribute("ID"), out var cfgId))
                                        {
                                        categoryFeatureGroupId = cfgId;  // Capture the CategoryFeatureGroup ID
                                        }
                                    break;
                                case "FeatureGroup":
                                    if (categoryId == 788 && int.TryParse(reader.GetAttribute("ID"), out var grpId))
                                        {
                                        featureGroupId = grpId;  // Capture the FeatureGroup ID
                                        }
                                    break;
                                case "Feature":
                                    if (categoryId == 788 && int.TryParse(reader.GetAttribute("ID"), out var fId))
                                        {
                                        featureId = fId;  // Capture the feature ID
                                        }
                                    break;
                                case "Name":
                                    if (categoryId == 788 && reader.GetAttribute("langid") == "1")
                                        {
                                        featureName = reader.GetAttribute("Value");  // Capture the feature name if langid is 1
                                        }
                                    break;
                                }
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "Feature" && categoryId == 788 && featureName != null)
                                {
                                var feature = new CategoryFeature
                                    {
                                    CategoryId = categoryId,
                                    IceCatFeatureGroupId = categoryFeatureGroupId,
                                    FeatureGroupId = featureGroupId,
                                    FeatureId = featureId,
                                    Name = featureName
                                    };
                                categoryFeatures.Add(feature);  // Add the feature to the list
                                featureName = null;  // Reset the feature name for the next feature
                                }
                            break;
                        }
                    }
                }
            }
        catch (Exception ex)
            {
            Console.WriteLine("Error processing the category features file: " + ex.Message);
            }


        return categoryFeatures;
        }






    private async Task SaveCategoryFeaturesToDatabase(List<CategoryFeature> categoryFeatures)
        {
        var response = await _categoryFeatureService.ImportCategoryFeaturesAsync(categoryFeatures);
        if (!response.Success)
            {
            // Handle errors
            Console.WriteLine("Failed to import category features: " + response.Message);
            }
        }



    #endregion




    #region Catalogs

    public async Task DownloadAndSaveAllCatalogs()
        {
     //   await  DownloadCatalogs();
       await UpdateAllCatalogColumnsAsync();
        }
    private async Task<List<Catalog>> DownloadCatalogs(int batchSize = 10000)
        {
        string localFilePath = @"F:\buurter\files.index.xml";
        List<Catalog> products = new List<Catalog>();

        try
            {
            using (FileStream fileStream = File.OpenRead(localFilePath))
            using (StreamReader reader = new StreamReader(fileStream))
                {
                XDocument doc = XDocument.Load(reader);
                var files = doc.Descendants("file")
           .Where(file =>
               (string)file.Attribute("On_Market") == "1" && // Only products on market
               DateTime.TryParseExact((string)file.Attribute("Updated"), "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime updatedDate) &&
               updatedDate.Year >= 2021 && updatedDate.Year <= 2025 // Updated between 2021 and 2025
           )
           .ToList();
                // var files = doc.Descendants("file").ToList();
                List<Catalog> batch = new List<Catalog>();

                foreach (var file in files)
                    {
                    string productXmlPath = (string)file.Attribute("path");
                    string productXmlUrl = "https://data.icecat.biz/" + productXmlPath;
                  //  XDocument productDoc = await DownloadProductXml(productXmlUrl);

                    var gtinElements = file.Descendants("EAN_UPC");
                    string selectedGtin = gtinElements
                        .Where(e => (int?)e.Attribute("IsApproved") == 1)
                        .Select(e => (string)e.Attribute("Value"))
                        .FirstOrDefault() ?? gtinElements.Select(e => (string)e.Attribute("Value")).FirstOrDefault();

                    var dateAddedStr = (string)file.Attribute("Date_Added");
                    var updatedStr = (string)file.Attribute("Updated");

                    DateTime createdDate = !string.IsNullOrEmpty(dateAddedStr)
                        ? DateTime.ParseExact(dateAddedStr, "yyyyMMddHHmmss", null)
                        : DateTime.Now;

                    DateTime lastModifiedDate = !string.IsNullOrEmpty(updatedStr)
                        ? DateTime.ParseExact(updatedStr, "yyyyMMddHHmmss", null)
                        : DateTime.Now;

                    var productDetails = new Catalog
                        {
                        IntegratedId = (int)file.Attribute("Product_ID"),
                        Code = (string)file.Attribute("Prod_ID"),
                        Integrated = true,
                        BrandId = (int)file.Attribute("Supplier_id"),
                        CategoryId = (int)file.Attribute("Catid"),
                        Name = (string)file.Attribute("Model_Name"),
                        EanNumber = selectedGtin,
                        Thumbnail = productXmlUrl,
                     /*   Thumbnail = (string)productDoc.Descendants("Product").Select(e => e.Attribute("ThumbPic")?.Value).FirstOrDefault(),
                        ShortDescription = productDoc.Descendants("SummaryDescription")
                            .Descendants("ShortSummaryDescription")
                            .Where(e => (string)e.Attribute("langid") == "1")
                            .Select(e => e.Value)
                            .FirstOrDefault(),
                        Description = productDoc.Descendants("SummaryDescription")
                            .Descendants("LongSummaryDescription")
                            .Where(e => (string)e.Attribute("langid") == "1")
                            .Select(e => e.Value)
                            .FirstOrDefault(),*/
                        CreatedDate = createdDate,
                        LastModifiedDate = lastModifiedDate
                        };

                    batch.Add(productDetails);

                    // Save the batch when it reaches the batch size
                    if (batch.Count >= batchSize)
                        {
                        await SaveCatalogs(batch);
                        products.AddRange(batch);
                        Console.WriteLine($"Saved {products.Count}/{files.Count} products.");
                        batch.Clear();
                        }
                    }

                // Save any remaining products
                if (batch.Count > 0)
                    {
                    await SaveCatalogs(batch);
                    products.AddRange(batch);
                    Console.WriteLine($"Saved final batch. Total products saved: {products.Count}");
                    }
                }
            }
        catch (Exception ex)
            {
            Console.WriteLine("Error reading local index file: " + ex.Message);
            }

        return products;
        }
    private async Task<XDocument> DownloadProductXml(string url)
        {
        try
            {
            using (HttpClient client = new HttpClient())
                {
                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

                string xmlContent = await client.GetStringAsync(url);
                return XDocument.Parse(xmlContent);
                }
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Error fetching product XML: {url}, {ex.Message}");
            return new XDocument();
            }
        }

    private async Task SaveCatalogs(List<Catalog> catalogs, int batchSize = 10000)
        {
        int totalCatalogs = catalogs.Count;
        int processed = 0;

        while (processed < totalCatalogs)
            {
            // Take a batch from the remaining items
            var batch = catalogs.Skip(processed).Take(batchSize).ToList();

            // Process the current batch asynchronously
            await _catalogService.AddCatalogsAsync(batch);

            // Update the count of processed items
            processed += batch.Count;

            // Optional: Log the progress or any other details
            Console.WriteLine($"Processed {processed}/{totalCatalogs} catalogs.");
            }
        }



    ///New working 

    public async Task<ServiceResponse<bool>> UpdateAllCatalogColumnsAsync(int batchSize = 1000, int maxDegreeOfParallelism = 50)
        {
        try
            {
            // Step 1: Get all catalogs
            var catalogsResponse = await _catalogService.GetCatalogsAsync();
            if (!catalogsResponse.Success || catalogsResponse.Data == null)
                {
                return new ServiceResponse<bool>
                    {
                    Success = false,
                    Message = "Failed to fetch catalogs."
                    };
                }

            var catalogs = catalogsResponse.Data;

            // Step 2: Process catalogs in batches
            for (int i = 0; i < catalogs.Count; i += batchSize)
                {
                var batch = catalogs.Skip(i).Take(batchSize).ToList();

                // Step 3: Parallel API calls to update columns
                var tasks = batch.Select(async catalog =>
                {
                    try
                        {
                        using (HttpClient client = new HttpClient())
                            {
                            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
                            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

                            string xmlContent = await client.GetStringAsync(catalog.Thumbnail); // Assuming Thumbnail stores API URL
                            XDocument productDoc = XDocument.Parse(xmlContent);

                            // Update columns from API response
                            catalog.Thumbnail = (string)productDoc.Descendants("Product").Select(e => e.Attribute("ThumbPic")?.Value).FirstOrDefault();
                            catalog.ShortDescription = productDoc.Descendants("SummaryDescription")
                                                                 .Descendants("ShortSummaryDescription")
                                                                 .Where(e => (string)e.Attribute("langid") == "1")
                                                                 .Select(e => e.Value)
                                                                 .FirstOrDefault();
                            catalog.Description = productDoc.Descendants("SummaryDescription")
                                                            .Descendants("LongSummaryDescription")
                                                            .Where(e => (string)e.Attribute("langid") == "1")
                                                            .Select(e => e.Value)
                                                            .FirstOrDefault();
                            }
                        }
                    catch (Exception ex)
                        {
                      Debug.WriteLine(ex, $"Error updating catalog {catalog.IntegratedId}: {ex.Message}");
                        }
                });

                // Step 4: Run tasks with limited parallelism
                await Task.WhenAll(tasks.Take(maxDegreeOfParallelism));

               await  _catalogService.BulkUpdateCatalogsAsync(batch);
                // Step 5: Bulk update to database
                //using (var transaction = await _context.Database.BeginTransactionAsync())
                //    {
                //    try
                //        {
                //        _context.Catalogs.UpdateRange(batch);
                //        await _context.SaveChangesAsync();
                //        await transaction.CommitAsync();
                //        Console.WriteLine($"Updated batch {i / batchSize + 1}/{(catalogs.Count / batchSize) + 1}");
                //        }
                //    catch (Exception ex)
                //        {
                //        await transaction.RollbackAsync();
                //        _logger.LogError(ex, "Error during bulk update.");
                //        }
                //    }
                }

            return new ServiceResponse<bool>
                {
                Success = true,
                Data = true,
                Message = "All catalogs updated successfully."
                };
            }
        catch (Exception ex)
            {
            Debug.WriteLine(ex, "Error occurred during catalog update.");
            return new ServiceResponse<bool>
                {
                Success = false,
                Message = "Error occurred while updating catalogs."
                };
            }
        }


    #endregion



    #region FeatureExtraPro

    private async Task<List<Feature>> DownloadFeaturesAsync(List<long> featureIds)
        {
        List<Feature> downloadedFeatures = new List<Feature>();

        try
            {
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential(_username, _password);
            byte[] data = await client.DownloadDataTaskAsync(_featuresUrl);

            using var compressedStream = new MemoryStream(data);
            using var decompressedStream = new MemoryStream();
            using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            gzipStream.CopyTo(decompressedStream);
            decompressedStream.Seek(0, SeekOrigin.Begin);

            var xmlContent = Encoding.UTF8.GetString(decompressedStream.ToArray());
            XDocument doc = XDocument.Parse(xmlContent);

            foreach (var featureId in featureIds)
                {
                var featureElement = doc.Descendants("Feature")
                    .FirstOrDefault(f => (int)f.Attribute("ID") == featureId);

                if (featureElement != null)
                    {
                    Feature feature = new Feature
                        {
                        Id = (int)featureElement.Attribute("ID"),
                        Name = featureElement.Elements("Names").Elements("Name")
                            .FirstOrDefault(n => (int)n.Attribute("langid") == 1)?.Value ?? "",

                        ClusterId = 1,
                        Translations = featureElement.Element("Names")?.Elements("Name")
                            .Where(n => new[] { 1, 2, 4 }.Contains((int)n.Attribute("langid"))) // English, Dutch, German
                            .Select(n => new FeatureTranslation
                                {
                                LanguageId = (int)n.Attribute("langid"),
                                TranslatedName = n.Value ?? string.Empty
                                })
                            .ToList() ?? new List<FeatureTranslation>(),
                        };

                    downloadedFeatures.Add(feature);
                    }
                }
            }
        catch (Exception ex)
            {
            
            }

        return downloadedFeatures;
        }


    #endregion


    #region FeatureGroupExtraPro

    private async Task<List<Cluster>> DownloadFeatureGroupsAsync(List<long> clusterIds)
        {
        List<Cluster> downloadedGroups = new();

        try
            {
            using WebClient client = new();
            client.Credentials = new NetworkCredential(_username, _password);
            byte[] data = await client.DownloadDataTaskAsync(_featureGroupsUrl);

            using var compressedStream = new MemoryStream(data);
            using var decompressedStream = new MemoryStream();
            using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            gzipStream.CopyTo(decompressedStream);
            decompressedStream.Seek(0, SeekOrigin.Begin);

            var xmlContent = Encoding.UTF8.GetString(decompressedStream.ToArray());
            XDocument doc = XDocument.Parse(xmlContent);

            foreach (var clusterId in clusterIds)
                {
                var fg = doc.Descendants("FeatureGroup")
                    .FirstOrDefault(x => (int)x.Attribute("ID") == clusterId);

                if (fg != null)
                    {
                    Cluster cluster = new()
                        {
                        Id = (int)fg.Attribute("ID"),
                        Name = fg.Elements("Name")
                            .FirstOrDefault(n => (int)n.Attribute("langid") == 1)?.Attribute("Value")?.Value ?? "Default",
                        Translations = fg.Elements("Name")
                            .Where(n => new[] { 1, 2, 4 }.Contains((int)n.Attribute("langid")))
                            .Select(n => new ClusterTranslation
                                {
                                LanguageId = (int)n.Attribute("langid"),
                                TranslatedName = n.Attribute("Value")?.Value ?? string.Empty
                                }).ToList()
                        };

                    downloadedGroups.Add(cluster);
                    }
                }
            }
        catch (Exception ex)
            {
            Console.WriteLine("Error downloading missing feature groups: " + ex.Message);
            }

        return downloadedGroups;
        }


    #endregion
    }

public class ProductDetails
    {
    public int ProductId { get; set; }
    public string ProdId { get; set; }
    public int SupplierId { get; set; }
    public int CategoryId { get; set; }
    public string ModelName { get; set; }
    public string OnMarket { get; set; }
    public string ProductXmlPath { get; set; }
    }

