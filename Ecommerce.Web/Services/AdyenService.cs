using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using Ecommerce.Shared.Dto;

namespace Ecommerce.Web.Services;

public interface IAdyenService
    {
    Task<JsonElement> GetPaymentMethodsAsync();
    Task<JsonElement> MakePaymentAsync(JsonElement paymentData);
    Task<JsonElement> HandleRedirectAsync(string redirectResult);
    }
public class AdyenService : IAdyenService
    {
    private readonly HttpClient _httpClient;
    private readonly AdyenSettings _settings;

    public AdyenService(HttpClient httpClient, IOptions<AdyenSettings> options)
        {
        _httpClient = httpClient;
        _settings = options.Value;
        }

    public async Task<JsonElement> GetPaymentMethodsAsync()
        {
        var payload = new
            {
            merchantAccount = _settings.MerchantAccount,
            countryCode = "NL",
            amount = new { currency = "EUR", value = 1000 }
            };

        return await PostToAdyen("paymentMethods", payload);
        }

    public async Task<JsonElement> MakePaymentAsync(JsonElement paymentData)
        {
        var payload = new
            {
            amount = new { currency = "EUR", value = 1000 },
            reference = Guid.NewGuid().ToString(),
            merchantAccount = _settings.MerchantAccount,
            paymentMethod = paymentData.GetProperty("paymentMethod"),
            returnUrl = _settings.ReturnUrl
            };

        return await PostToAdyen("payments", payload);
        }

    public async Task<JsonElement> HandleRedirectAsync(string redirectResult)
        {
        var payload = new { details = new { redirectResult } };
        return await PostToAdyen("payments/details", payload);
        }

    private async Task<JsonElement> PostToAdyen(string path, object payload)
        {
        var url = $"https://checkout-test.adyen.com/v70/{path}";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        return JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        }
    }

public static class StaticServiceProvider
    {
    private static IServiceProvider? _serviceProvider;

    public static void Configure(IServiceProvider serviceProvider)
        {
        _serviceProvider = serviceProvider;
        }

    public static T GetService<T>() where T : notnull
        {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not initialized.");
        return _serviceProvider.GetRequiredService<T>();
        }
    }
