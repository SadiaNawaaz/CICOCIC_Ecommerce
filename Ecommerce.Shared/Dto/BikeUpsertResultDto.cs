using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Dto;
public sealed class BikeUpsertResultDto
    {
    public bool Ok { get; set; }
    public string? Message { get; set; }

    public long ProductId { get; set; }
    public long VariantId { get; set; }

    public bool UserExists { get; set; }          // ownerUserId.HasValue
    public bool Published { get; set; }           // variant.Publish
    public bool ClaimEmailSent { get; set; }      // email sent when user didn’t exist

    public string? PublicUrl { get; set; }        // e.g., https://your-site/Product?Id=38
    public string? NextAction { get; set; }       // e.g., "OpenPublicUrl" or "CheckEmail"
    }


public sealed class ApiResult<T>
    {
    public bool Success { get; init; }
    public string? Message { get; init; }
    public string? Code { get; init; } // optional machine code, e.g. "OK", "VALIDATION_FAILED"
    public T? Data { get; init; }
    public Dictionary<string, string[]>? Errors { get; init; } // field-wise errors

    public static ApiResult<T> Ok(T data, string? message = null, string? code = "OK")
        => new() { Success = true, Message = message, Code = code, Data = data };
    public static ApiResult<T> Fail(string message, string? code = "ERROR", Dictionary<string, string[]>? errors = null)
        => new() { Success = false, Message = message, Code = code, Errors = errors };
    }
