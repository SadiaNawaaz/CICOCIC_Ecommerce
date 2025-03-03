using Microsoft.Extensions.Localization;

namespace Ecommerce.Web.Services;

using Microsoft.JSInterop;
using System.Globalization;

public class CultureStateService
    {
    private readonly IJSRuntime _jsRuntime;
    public event Action? OnChange;
    private string _currentCulture = "en-US";

    public string CurrentCulture
        {
        get => _currentCulture;
        set
            {
            if (_currentCulture != value)
                {
                _currentCulture = value;
                CultureInfo.CurrentCulture = new CultureInfo(value);
                CultureInfo.CurrentUICulture = new CultureInfo(value);
                OnChange?.Invoke();
                }
            }
        }

    public async Task LoadCultureFromCookie()
        {
        var culture = await _jsRuntime.InvokeAsync<string>("getCookie", "AspNetCore.Culture") ?? "en-US";
        CurrentCulture = culture;
        }

    public async Task SetCultureInCookie(string culture)
        {
        await _jsRuntime.InvokeVoidAsync("setCookie", "AspNetCore.Culture", culture, 365);
        }
    }

