using Ecommerce.Mailer;
using Ecommerce.Mailer.DTOs;
using Ecommerce.Shared.Context;
using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Entities.Colors;
using Ecommerce.Shared.Entities.ModelYears;
using Ecommerce.Shared.Entities.Products;
using Ecommerce.Shared.Entities.ProductVariants;
using Ecommerce.Shared.Entities.Roles;
using Ecommerce.Shared.Entities.Sizes;
using Ecommerce.Shared.Enums;
using Ecommerce.Shared.Services.Products;
using Ecommerce.Shared.Services.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.Integrations;
public interface IBikeListingIngestionService
    {
    Task<BikeUpsertResultDto> UpsertAsync( BikeListingUpsertRequestDto dto,CancellationToken ct = default);
    Task<ResetPasswordGateResultDto> GetResetPasswordGateAsync(long productId, string email, CancellationToken ct = default);
    Task<ResetPasswordGateResultDto> SetPasswordAndClaimAsync(long productId, string email, string password, CancellationToken ct = default);
    }

public sealed class BikeListingIngestionService : IBikeListingIngestionService
    {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BikeListingIngestionService> _log;
    private readonly IMailerServices _mailer;
    private readonly string _baseUrl = "https://registerbyasset.com";
    private readonly string _WebbaseUrl = "https://cicocic.com";
    private readonly IImageStorage _imageStorage;
    public BikeListingIngestionService( ApplicationDbContext context,ILogger<BikeListingIngestionService> log, IMailerServices mailer, IImageStorage imageStorage)
        {
        _context = context;
        _log = log;
        _mailer = mailer;
        _imageStorage = imageStorage;
        }
    private static string? NormalizeEmail(string? e)=> string.IsNullOrWhiteSpace(e) ? null : e.Trim().ToLowerInvariant();

    public async  Task<BikeUpsertResultDto> UpsertAsync(BikeListingUpsertRequestDto dto, CancellationToken ct)
        {
        var refs = await ResolveRefIdsAsync(dto, ct);
    
        return new BikeUpsertResultDto
            {
            Ok = true,
            Message = (refs.UserExists)
                 ? "Listing published successfully."
                 : "Listing created. Please check your confirmation email to activate.",
            ProductId = refs.ProductId,
            VariantId = refs.VariantId,
            UserExists = refs.UserExists,
            Published = refs.Published ,
            ClaimEmailSent = !(refs.UserExists), 
            PublicUrl = $"{_WebbaseUrl}/Product?Id={refs.VariantId}",
            NextAction = (refs.UserExists) ? "OpenPublicUrl" : "CheckEmail"
            };

        }



    public async Task<ResetPasswordGateResultDto> GetResetPasswordGateAsync(long variantId, string email, CancellationToken ct)
        {
        var norm = NormalizeEmail(email);
        if (string.IsNullOrWhiteSpace(norm))
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.MissingEmail, Message = "Email required." };

        var exists = await _context.ProductVariants.AnyAsync(p => p.Id == variantId, ct);
        if (!exists)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.ProductNotFound, Message = "Listing not found." };

        var link = await _context.IntegrationUserLinks.FirstOrDefaultAsync(x => x.VariantId == variantId, ct);
        if (link is null)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.LinkNotFound, Message = "No claim record for this listing." };

        if (!string.Equals(link.Email ?? "", norm, StringComparison.OrdinalIgnoreCase))
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.EmailMismatch, Message = "Email does not match this listing." };

        if (link.EcommerceUserId > 0)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.AlreadyClaimed, Message = "This listing is already claimed." };

        return new ResetPasswordGateResultDto
            {
            Allow = true,
            Status = ResetPasswordStatus.Allow,
            Message = "Proceed to set password.",
            VariantId = variantId,
            PublicUrl = $"{_WebbaseUrl}/Product?Id={variantId}" // <- storefront by variant
            };
        }



    public async Task<ResetPasswordGateResultDto> SetPasswordAndClaimAsync(long variantId, string email, string password, CancellationToken ct)
        {
        var norm = NormalizeEmail(email);
        if (string.IsNullOrWhiteSpace(norm))
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.MissingEmail, Message = "Email required." };
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.MissingEmail, Message = "Password must be at least 8 characters." };

        var link = await _context.IntegrationUserLinks.FirstOrDefaultAsync(x => x.VariantId == variantId, ct);
        if (link is null)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.LinkNotFound, Message = "No claim record for this listing." };
        if (!string.Equals(link.Email ?? "", norm, StringComparison.OrdinalIgnoreCase))
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.EmailMismatch, Message = "Email does not match this listing." };
        if (link.EcommerceUserId > 0)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.AlreadyClaimed, Message = "This listing is already claimed." };

        var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.Id == variantId, ct);
        if (variant is null)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.ProductNotFound, Message = "Listing not found." };

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == norm, ct);
        if (user is null)
            {
            user = new User
                {
                Email = norm,
                UserName = norm,
                FirstName = link.FirstName,
                LastName = link.LastName,
                Approved = true,
                IsAgent = true,
                Password = password,
                CreatedDate = DateTime.UtcNow
                };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);
            }

        var agentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Agent", ct);
        if (agentRole is null)
            {
            agentRole = new Role { Name = "Agent" };
            _context.Roles.Add(agentRole);
            await _context.SaveChangesAsync(ct);
            }

        var hasRole = await _context.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == agentRole.Id, ct);
        if (!hasRole)
            {
            _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = agentRole.Id });
            await _context.SaveChangesAsync(ct);
            }

        variant.Publish = true;
        if (variant.CreatedBy == null) variant.CreatedBy = user.Id;

        link.EcommerceUserId = user.Id;
        await _context.SaveChangesAsync(ct);

        return new ResetPasswordGateResultDto
            {
            Allow = true,
            Status = ResetPasswordStatus.Allow,
            Message = "Password set. Listing activated.",
            VariantId = variantId,
            PublicUrl = $"{_WebbaseUrl}/Product?Id={variantId}"
            };
        }



    public async Task<ResolvedRefIds> ResolveRefIdsAsync(BikeListingUpsertRequestDto dto, CancellationToken ct = default)
        {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.BrandName))
            throw new ArgumentException("Please select a Brand before publishing.", nameof(dto.BrandName));

        if (string.IsNullOrWhiteSpace(dto.ModelName))
            throw new ArgumentException("Please enter a Model before publishing.", nameof(dto.ModelName));

        if (string.IsNullOrWhiteSpace(dto.CategoryName))
            throw new ArgumentException("Please select a Category before publishing.", nameof(dto.CategoryName));

        if (string.IsNullOrWhiteSpace(dto.ColorPrimary))
            throw new ArgumentException("Please select a Primary color before publishing.", nameof(dto.ColorPrimary));

        if (string.IsNullOrWhiteSpace(dto.Size))
            throw new ArgumentException("Please select a Size before publishing.", nameof(dto.Size));

        if (string.IsNullOrWhiteSpace(dto.Year))
            throw new ArgumentException("Please select a Year before publishing.", nameof(dto.Year));


        using var trx = await _context.Database.BeginTransactionAsync(ct);

        // Normalize (don’t compare slug to Name)
        string N(string s) => s.Trim();
        var brandName = N(dto.BrandName);
        var categoryName = N(dto.CategoryName);
        var colorName = N(dto.ColorPrimary);
        var sizeName = N(dto.Size);
        var yearValue = TryParseYear(dto.Year);



        var externalUserId = dto.ExternalUserId;
        var ownerUserId = await TryResolveOwnerUserIdAsync(dto.Email, externalUserId, ct);
        bool shouldPublish = ownerUserId.HasValue;
        // -- Brand
        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Name == brandName, ct);
        if (brand is null)
            {
            brand = new Brand { Name = brandName ,MarkBrand=true };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync(ct);
            }

        // ---- Category
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName, ct);
        if (category is null)
            {
            category = new Category { Name = categoryName };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(ct);
            }

        // -- Color
        var color = await _context.Colors.FirstOrDefaultAsync(c => c.Name == colorName, ct);
        if (color is null)
            {
            color = new GeneralColor { Name = colorName ,HexCode=""};
            _context.Colors.Add(color);
            await _context.SaveChangesAsync(ct);
            }

        // -- Size
        var size = await _context.Sizes.FirstOrDefaultAsync(s => s.Name == sizeName, ct);
        if (size is null)
            {
            size = new GeneralSize { Name = sizeName };
            _context.Sizes.Add(size);
            await _context.SaveChangesAsync(ct);
            }

        // -- Year 
        long? yearId = null;
        if (yearValue.HasValue)
            {
            var yearRow = await _context.ModelYears.FirstOrDefaultAsync(y => y.Year == yearValue.Value, ct);
            if (yearRow is null)
                {
                yearRow = new ModelYear { Year = yearValue.Value };
                _context.ModelYears.Add(yearRow);
                await _context.SaveChangesAsync(ct);
                }
            yearId = yearRow.Id;
            }

        // ---------------- Product ----------------
    

        var title = BuildTitle(brandName, dto.ModelName, yearValue, colorName, "");
        var slug = ToSlug(title, shortId: Guid.NewGuid().ToString("N")[..6]);


        var product = new Product
            {
            BrandId = brand.Id,
            CategoryId = category.Id,
            Name = title,                 
            Code = dto.ModelName,       
            Slug = slug,
            Price = dto.Price,        
            ShortDescription = dto.ShortDescription,
            Description = dto.Description,
            //EanNumber = dto.EanNumber,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = ownerUserId,
            };
        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct); 

        // -- Variant --
        var variant = new ProductVariant
            {
            ProductId = product.Id,
            Name = dto.ModelName,          
            GeneralSizeId = size.Id,
            GeneralColorId = color.Id,
            ModelYearId = yearId,        
            Price = dto.Price,
            Sku = string.IsNullOrWhiteSpace(dto.Productnumber) ? $"CICO-{product.Id:000000}" : dto.Productnumber.Trim(),
            SSN = dto.SerialNumber ?? dto.FrameNumber, 
            Description = dto.Description,
            //Thumbnail = dto.ThumbnailUrl,
            Publish = shouldPublish,
            variantType = (int)Enums.VariantType.Serial,
            Sold = 0,                       
            CreatedDate = DateTime.UtcNow,
            CreatedBy = ownerUserId,
            };
        _context.ProductVariants.Add(variant);
        await _context.SaveChangesAsync(ct);
        await UpsertVariantAttributesFromDtoAsync(product.Id, variant.Id, ownerUserId, dto, ct);


        if (dto.Images != null && dto.Images.Count > 0)
            {
            int order = 0;
            foreach (var img in dto.Images)
                {
          
                byte[] bytes = Convert.FromBase64String(img.DataBase64);
                if (bytes is null || bytes.Length == 0) continue;

                var ext = PickExt(img.FileName ?? img.ContentType);
                var unique = $"{Guid.NewGuid():N}{ext}";
                var baseName = Path.GetFileNameWithoutExtension(unique);
                var originalUrl = await _imageStorage.SaveOriginalAsync(variant.Id, unique, bytes, ct);
               

                // persist DB record
                var dbImage = new ProductVariantImages
                    {
                    ProductVariantId = variant.Id,
                    ImageUrl = originalUrl,
                    ImageName = unique,
                    ImageByte = null,            
                    Order = order++,
                    IsDeleted = false,
                    IsCloned = false,
                    CreatedDate = DateTime.UtcNow
                    };
                _context.productVariantImages.Add(dbImage);
                }

            await _context.SaveChangesAsync(ct);
            }





        if (!ownerUserId.HasValue)
            await UpsertIntegrationUserLinkAndMaybeNotifyAsync(variant.Id, externalUserId, dto.Email, dto.FirstName, dto.LastName, ownerUserId, ct);

        await trx.CommitAsync(ct);

        return new ResolvedRefIds
            {
            BrandId = brand.Id,
            CategoryId = category.Id,
            ColorId = color.Id,
            SizeId = size.Id,
            YearId = yearId ?? 0,
            BrandSlug = Slug(brandName),
            CategorySlug = Slug(categoryName),
            ColorSlug = Slug(colorName),
            SizeSlug = Slug(sizeName),
            YearLabel = dto.Year.Trim(),

            // NEW:
            ProductId = product.Id,
            VariantId = variant.Id,
            UserExists = ownerUserId.HasValue,
            Published = shouldPublish
            };


        }


    // ---------- helpers ----------

    private static string Slug(string s)
        {
        if (string.IsNullOrWhiteSpace(s)) return string.Empty;
        s = s.Trim().ToLowerInvariant();
        var chars = s.Select(ch => char.IsLetterOrDigit(ch) ? ch : '-').ToArray();
        var slug = new string(chars);
        while (slug.Contains("--")) slug = slug.Replace("--", "-");
        return slug.Trim('-');
        }

    private static int? TryParseYear(string s)
        => int.TryParse(s, out var y) && y >= 1900 && y <= 2100 ? y : (int?)null;

    private static string BuildTitle(
        string brandName,
        string modelName,
        int? year = null,
        string? colorName = null,
        string? wheelSize = null,
        string? extraRight = null
    )
        {
        string N(string? s) => (s ?? string.Empty).Trim();

        var leftParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(brandName)) leftParts.Add(N(brandName));
        if (!string.IsNullOrWhiteSpace(modelName)) leftParts.Add(N(modelName));
        if (year.HasValue) leftParts.Add($"({year.Value})");

        var rightParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(colorName)) rightParts.Add(N(colorName));

        var ws = NormalizeWheelSize(wheelSize);
        if (!string.IsNullOrWhiteSpace(ws)) rightParts.Add(ws);

        if (!string.IsNullOrWhiteSpace(extraRight)) rightParts.Add(N(extraRight));

        var left = string.Join(" ", leftParts).Trim();
        var right = rightParts.Count > 0 ? " — " + string.Join(" ", rightParts) : string.Empty;

        return (left + right).Trim();
        }

    private static string ToSlug(string input, string? shortId = null, int maxLength = 96)
        {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // remove accents (é -> e)
        string s = RemoveDiacritics(input).ToLowerInvariant();

        // spaces/underscores -> '-'
        s = System.Text.RegularExpressions.Regex.Replace(s, @"[\s_]+", "-");

        // keep a-z, 0-9, '-'
        s = System.Text.RegularExpressions.Regex.Replace(s, @"[^a-z0-9\-]", "");

        // collapse '--'
        s = System.Text.RegularExpressions.Regex.Replace(s, @"\-{2,}", "-").Trim('-');

        // trim length
        if (s.Length > maxLength) s = s[..maxLength].Trim('-');

        // optional stable suffix for uniqueness
        if (!string.IsNullOrWhiteSpace(shortId))
            {
            var safe = System.Text.RegularExpressions.Regex.Replace(shortId.ToLowerInvariant(), @"[^a-z0-9]", "");
            if (!string.IsNullOrEmpty(safe)) s = $"{s}-{safe}".Trim('-');
            }

        return s;
        }

    private static string NormalizeWheelSize(string? raw)
        {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        var m = System.Text.RegularExpressions.Regex.Match(raw.Trim(), @"\d+(\.\d+)?");
        if (!m.Success) return string.Empty;
        var num = m.Value.TrimEnd('.');
        return $"{num}\""; // add inch mark, e.g., 29"
        }

    private static string RemoveDiacritics(string text)
        {
        var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder(capacity: normalized.Length);
        foreach (var c in normalized)
            {
            var cat = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (cat != System.Globalization.UnicodeCategory.NonSpacingMark) sb.Append(c);
            }
        return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

    private async Task<long?> TryResolveOwnerUserIdAsync(string? email, int  externalUserId, CancellationToken ct)
        {
        var norm = NormalizeEmail(email);
        if (!string.IsNullOrWhiteSpace(norm))
            {
            var byEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == norm, ct);
            if (byEmail is not null) return byEmail.Id;
            }
        var prior = await _context.IntegrationUserLinks
            .Where(x => x.ExternalUserId == externalUserId && x.EcommerceUserId != null)
            .Select(x => x.EcommerceUserId)
            .Cast<long?>()
            .FirstOrDefaultAsync(ct);
        return prior;
        }

    private async Task UpsertIntegrationUserLinkAndMaybeNotifyAsync(long variantId, int  externalUserId, string? email, string? firstName, string? lastName, long? ownerUserId, CancellationToken ct)
        {
        var norm = NormalizeEmail(email);
        var link = await _context.IntegrationUserLinks.FirstOrDefaultAsync(x => x.VariantId == variantId, ct);
        if (link is null)
            {
            link = new IntegrationUserLink
                {
                VariantId = variantId,
                ExternalUserId = externalUserId,
                Email = norm,
                FirstName = firstName,
                LastName = lastName
                };
            _context.IntegrationUserLinks.Add(link);
            }
        else
            {
            link.ExternalUserId = externalUserId;
            link.Email = norm;
            link.FirstName = firstName;
            link.LastName = lastName;
            }
        await _context.SaveChangesAsync(ct);

    
        if (!ownerUserId.HasValue && !string.IsNullOrWhiteSpace(norm))
            {
            var toName = $"{firstName} {lastName}".Trim();
            var url = $"{_baseUrl}/auth/set-password?variantId={variantId}&email={email}";

            var mail = new SendEmail
                {
                To = new List<EmailAddress> { new EmailAddress { Email = norm } },
                Subject = "Set your password to manage your listing",
                Body = $"<p>Hello {toName},</p><p>Set your password to claim and manage your listing.</p><p><a href=\"{url}\">Set Password</a></p>"
                };

            await _mailer.Init();
            await _mailer.SendEmailAsync(mail, "administrator@registerbyasset.com");
            }










        }

    private static string PickExt(string? fileNameOrContentType)
        {
        if (string.IsNullOrWhiteSpace(fileNameOrContentType)) return ".png";
        var s = fileNameOrContentType.ToLowerInvariant();
        if (s.Contains("png") || s.EndsWith(".png")) return ".png";
        if (s.Contains("jpeg") || s.EndsWith(".jpeg") || s.Contains("jpg") || s.EndsWith(".jpg")) return ".jpg";
        if (s.Contains("webp") || s.EndsWith(".webp")) return ".webp";
        if (Path.HasExtension(s)) return Path.GetExtension(s);
        return ".png";
        }



    private async Task UpsertVariantAttributesFromDtoAsync(
        long productId, long variantId, long? createdBy, BikeListingUpsertRequestDto dto, CancellationToken ct)
        {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // basics
        AddPair(dict, "BikeModel", dto.ModelName);
        AddPair(dict, "FrameNumber", dto.FrameNumber);
        AddPair(dict, "SerialNumber", dto.SerialNumber);
        AddPair(dict, "Productnumber", dto.Productnumber);
        AddPair(dict, "Currency", dto.Currency);
        AddPair(dict, "Year", dto.Year);
        AddPair(dict, "Price", dto.Price.ToString());

        // colors / type / audience
 
        AddPair(dict, "FrameColorPrimary", dto.ColorPrimary);
        AddPair(dict, "FrameColorSecondary", dto.ColorSecondary);
        AddPair(dict, "Audience", dto.Audience);

        // sizing & mechanics
        AddPair(dict, "WheelSize", dto.WheelSize);
        AddPair(dict, "FrameHeight", dto.FrameHeight);
        AddPair(dict, "FrameMaterial", dto.FrameMaterial);
        AddPair(dict, "BrakeType", dto.BrakeType);
        AddPair(dict, "GearType", dto.GearType);
        AddPair(dict, "DriveType", dto.DriveType);
        AddPair(dict, "Gears", dto.Gears?.ToString());

        // security / locks / keys
        AddPair(dict, "LockModel", dto.LockModel);
        AddPair(dict, "KeyNumber", dto.KeyNumber);

        // powertrain / electric
        if(dto.PropulsionType)
            {
            AddPair(dict, "AccelerationType", dto.AccelerationType);
            AddPair(dict, "MileageKm", dto.MileageKm?.ToString());
            AddPair(dict, "BatteryNumber", dto.BatteryNumber);
            AddPair(dict, "BatteryCapacityWh", dto.BatteryCapacity);
            AddPair(dict, "BrandEngine", dto.BrandEngine);
            AddPair(dict, "EngineType", dto.EngineType);
            AddPair(dict, "EngineNumber", dto.EngineNumber);
            AddPair(dict, "ChargerSerialNumber", dto.ChargerSerialNumber);
            AddPair(dict, "DisplaySerialNumber", dto.DisplaySerialNumber);
            }
  



        var json = System.Text.Json.JsonSerializer.Serialize(dict);

        var row = await _context.VariantAttributes
            .FirstOrDefaultAsync(x => x.ProductVariantId == variantId, ct);

        if (row is null)
            {
            row = new VariantAttributes
                {
                ProductId = productId,
                ProductVariantId = variantId,
                Data = json,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
                };
            _context.VariantAttributes.Add(row);
            }
        else
            {
            row.ProductId = productId;
            row.Data = json;
            row.LastModifiedBy = createdBy;
            row.LastModifiedDate = DateTime.UtcNow;
            }

        await _context.SaveChangesAsync(ct);
        }

    private static void AddPair(Dictionary<string, string> d, string key, string? val)
        {
        if (!string.IsNullOrWhiteSpace(val)) d[key] = val.Trim();
        }

    }



public interface IImageStorage
    {
    Task<string> SaveOriginalAsync(long productId, string fileName, byte[] data, CancellationToken ct);
 
    }
