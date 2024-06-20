using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Ecommerce.Admin.Services;

public class ImageResizeService
{
    public async Task<string> ResizeImage(IBrowserFile file, int maxWidth, int maxHeight)
    {

       try
        {
            using var image = await Image.LoadAsync(file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024));
            float widthRatio = (float)maxWidth / image.Width;
            float heightRatio = (float)maxHeight / image.Height;
            float resizeRatio = Math.Min(widthRatio, heightRatio);
            int newWidth = (int)(image.Width * resizeRatio);
            int newHeight = (int)(image.Height * resizeRatio);
            var resizedImage = image.Clone(x => x.Resize(newWidth, newHeight));

            using var ms = new MemoryStream();
            await resizedImage.SaveAsPngAsync(ms);

            return Convert.ToBase64String(ms.ToArray());
        }
     catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<string> ResizeImage(Stream imageStream, string contentType, int maxWidth, int maxHeight)
    {
        try
        {
            using var image = await Image.LoadAsync(imageStream);
            float widthRatio = (float)maxWidth / image.Width;
            float heightRatio = (float)maxHeight / image.Height;
            float resizeRatio = Math.Min(widthRatio, heightRatio);
            int newWidth = (int)(image.Width * resizeRatio);
            int newHeight = (int)(image.Height * resizeRatio);
            var resizedImage = image.Clone(x => x.Resize(newWidth, newHeight));

            using var ms = new MemoryStream();
            await resizedImage.SaveAsPngAsync(ms);

            return $"data:{contentType};base64,{Convert.ToBase64String(ms.ToArray())}";
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}