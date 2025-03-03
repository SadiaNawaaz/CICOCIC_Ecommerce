namespace Ecommerce.Web;

public class AppSettings
{
    public string BaseUrl { get; set; }
}

public static class UrlHelper
{
    private static string _baseUrl;

    public static void Initialize(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public static string GetImageUrl(long productId, string imageUrl)
    {
        if(imageUrl==null)
            {
            return "";
            }
        if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
            {
            return imageUrl;
            }

        return $"{_baseUrl}/ProductVariants/{productId}/{imageUrl}";
    }

    public static string GetBrandImageUrl(long productId, string imageUrl)
    {

        if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
            {
            return imageUrl; 
            }

        return $"{_baseUrl}/Brands/{productId}/{imageUrl}";
    }

    public static string GetSliderImageUrl(long productId, string imageUrl)
    {
        return $"{_baseUrl}/Slider/{productId}/{imageUrl}";
    }
    public static string GetMenuIconImageUrl(long productId, string imageUrl)
    {
        if (!string.IsNullOrEmpty(imageUrl))
            {
            return $"{_baseUrl}/Category/{productId}/{imageUrl}";
            }
        else
            {
            return "/assets/img/icon/icon-star.png";
            }
        
    }

    public static string GetVariantImageUrl(long VariantId, string imageUrl)
        {
        if (!string.IsNullOrEmpty(imageUrl) && (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://")))
            {
            return imageUrl;
            }

        return $"{_baseUrl}/ProductVariants/{VariantId}/{imageUrl}";
        }


    public static string GetPopularBrandImageUrl(long Id, string imageUrl)
    {
        return $"{_baseUrl}/PopularBrand/{Id}/{imageUrl}";
    }

    public static string GetPopularCategoriesImageUrl(long Id, string imageUrl)
    {
        return $"{_baseUrl}/PopularCategory/{Id}/{imageUrl}";
    }

    public static string GetLatestCategoryImageUrl(long Id, string imageUrl)
    {
        return $"{_baseUrl}/CategoryConfig/{Id}/{imageUrl}";
    }
}

