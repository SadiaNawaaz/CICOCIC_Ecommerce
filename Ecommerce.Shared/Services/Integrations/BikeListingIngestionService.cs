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
    Task<BikeListingUpsertRequestDto> UpsertAsync( BikeListingUpsertRequestDto dto,CancellationToken ct = default);
    Task<ResetPasswordGateResultDto> GetResetPasswordGateAsync(long productId, string email, CancellationToken ct = default);
    Task<ResetPasswordGateResultDto> SetPasswordAndClaimAsync(long productId, string email, string password, CancellationToken ct = default);
    }

public sealed class BikeListingIngestionService : IBikeListingIngestionService
    {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BikeListingIngestionService> _log;
    private readonly IMailerServices _mailer;
    private readonly string _baseUrl = "https://registerbyasset.com";

    public BikeListingIngestionService( ApplicationDbContext context,ILogger<BikeListingIngestionService> log, IMailerServices mailer)
        {
        _context = context;
        _log = log;
        _mailer = mailer;
        }
    private static string? NormalizeEmail(string? e)=> string.IsNullOrWhiteSpace(e) ? null : e.Trim().ToLowerInvariant();

    public async  Task<BikeListingUpsertRequestDto>UpsertAsync(BikeListingUpsertRequestDto dto, CancellationToken ct)
        {
        var refs = await ResolveRefIdsAsync(dto, ct);
        dto.Ok = true; 
        return dto;
    
        }



    public async Task<ResetPasswordGateResultDto> GetResetPasswordGateAsync(long productId, string email, CancellationToken ct)
        {
        var norm = NormalizeEmail(email);
        if (string.IsNullOrWhiteSpace(norm))
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.MissingEmail, Message = "Email required." };

        var productExists = await _context.Products.AnyAsync(p => p.Id == productId, ct);
        if (!productExists)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.ProductNotFound, Message = "Listing not found." };

        var link = await _context.IntegrationUserLinks.FirstOrDefaultAsync(x => x.ProductId == productId, ct);
        if (link is null)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.LinkNotFound, Message = "No claim record for this listing." };

        if (!string.Equals(link.Email ?? "", norm, StringComparison.OrdinalIgnoreCase))
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.EmailMismatch, Message = "Email does not match this listing." };

        if (link.EcommerceUserId > 0)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.AlreadyClaimed, Message = "This listing is already claimed." };

        return new ResetPasswordGateResultDto { Allow = true, Status = ResetPasswordStatus.Allow, Message = "Proceed to set password." };
        }


    public async Task<ResetPasswordGateResultDto> SetPasswordAndClaimAsync(long productId, string email, string password, CancellationToken ct)
        {
        var norm = NormalizeEmail(email);
        if (string.IsNullOrWhiteSpace(norm))
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.MissingEmail, Message = "Email required." };
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.MissingEmail, Message = "Password must be at least 8 characters." };

        var link = await _context.IntegrationUserLinks.FirstOrDefaultAsync(x => x.ProductId == productId, ct);
        if (link is null)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.LinkNotFound, Message = "No claim record for this listing." };
        if (!string.Equals(link.Email ?? "", norm, StringComparison.OrdinalIgnoreCase))
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.EmailMismatch, Message = "Email does not match this listing." };
        if (link.EcommerceUserId > 0)
            return new ResetPasswordGateResultDto { Allow = false, Status = ResetPasswordStatus.AlreadyClaimed, Message = "This listing is already claimed." };

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);
        if (product is null)
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

        product.CreatedBy = user.Id;

        var variants = await _context.ProductVariants.Where(v => v.ProductId == productId).ToListAsync(ct);
        foreach (var v in variants)
            {
            v.Publish = true;
            if (v.CreatedBy == null) v.CreatedBy = user.Id;
            }

        link.EcommerceUserId = user.Id;
        await _context.SaveChangesAsync(ct);

        return new ResetPasswordGateResultDto { Allow = true, Status = ResetPasswordStatus.Allow, Message = "Password set. Listing activated." };
        }



    public async Task<ResolvedRefIds> ResolveRefIdsAsync(BikeListingUpsertRequestDto dto, CancellationToken ct = default)
        {
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.BrandName)) throw new ArgumentException("BrandName required");
        if (string.IsNullOrWhiteSpace(dto.ModelName)) throw new ArgumentException("ModelName required");
        if (string.IsNullOrWhiteSpace(dto.CategoryName)) throw new ArgumentException("CategoryName required");
        if (string.IsNullOrWhiteSpace(dto.ColorName)) throw new ArgumentException("ColorName required");
        if (string.IsNullOrWhiteSpace(dto.Size)) throw new ArgumentException("Size required");
        if (string.IsNullOrWhiteSpace(dto.Year)) throw new ArgumentException("Year required");

        using var trx = await _context.Database.BeginTransactionAsync(ct);

        // Normalize (don’t compare slug to Name)
        string N(string s) => s.Trim();
        var brandName = N(dto.BrandName);
        var categoryName = N(dto.CategoryName);
        var colorName = N(dto.ColorName);
        var sizeName = N(dto.Size);
        var yearValue = TryParseYear(dto.Year);



        var externalUserId = dto.ExternalUserId;
        var ownerUserId = await TryResolveOwnerUserIdAsync(dto.Email, externalUserId, ct);

        // ---- Brand
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

        // ---- Color  (use the same entity as FK table)
        var color = await _context.Colors.FirstOrDefaultAsync(c => c.Name == colorName, ct);
        if (color is null)
            {
            color = new GeneralColor { Name = colorName ,HexCode=""};
            _context.Colors.Add(color);
            await _context.SaveChangesAsync(ct);
            }

        // ---- Size
        var size = await _context.Sizes.FirstOrDefaultAsync(s => s.Name == sizeName, ct);
        if (size is null)
            {
            size = new GeneralSize { Name = sizeName };
            _context.Sizes.Add(size);
            await _context.SaveChangesAsync(ct);
            }

        // ---- Year (optional lookup)
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

        // ---------------- Variant ----------------
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
            Publish = false,
            variantType = (int)Enums.VariantType.Serial,
            Sold = 0,                       
            CreatedDate = DateTime.UtcNow,
            CreatedBy = ownerUserId,
            };
        _context.ProductVariants.Add(variant);
        await _context.SaveChangesAsync(ct);

        if (!ownerUserId.HasValue)
            await UpsertIntegrationUserLinkAndMaybeNotifyAsync(product.Id, externalUserId, dto.Email, dto.FirstName, dto.LastName, ownerUserId, ct);

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
            YearLabel = dto.Year.Trim()
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
    // ---------- Title & Slug (inline helpers) ----------
    private static string BuildTitle(
        string brandName,
        string modelName,
        int? year = null,
        string? colorName = null,
        string? wheelSize = null,
        string? extraRight = null // e.g., "Step-Through"
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

    private async Task UpsertIntegrationUserLinkAndMaybeNotifyAsync(long productId, int  externalUserId, string? email, string? firstName, string? lastName, long? ownerUserId, CancellationToken ct)
        {
        var norm = NormalizeEmail(email);
        var link = await _context.IntegrationUserLinks.FirstOrDefaultAsync(x => x.ProductId == productId, ct);
        if (link is null)
            {
            link = new IntegrationUserLink
                {
                ProductId = productId,
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
            var url = $"{_baseUrl}/auth/set-password?productId={productId}";

            var mail = new SendEmail
                {
                To = new List<EmailAddress> { new EmailAddress { Email = norm } },
                Subject = "Set your password to manage your listing",
                Body = $"<p>Hello {toName},</p><p>Set your password to claim and manage your listing.</p><p><a href=\"{url}\">Set Password</a></p><p>If the button doesn’t work, copy this URL:<br/>{url}</p>"
                };

            await _mailer.Init();
            await _mailer.SendEmailAsync(mail, "administrator@registerbyasset.com");
            }










        }



    

    }

