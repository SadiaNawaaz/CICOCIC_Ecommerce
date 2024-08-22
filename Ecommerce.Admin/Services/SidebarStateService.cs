using Blazored.SessionStorage;

namespace Ecommerce.Admin.Services;

public class SidebarStateService
{
    private readonly ISessionStorageService _sessionStorage;
    private HashSet<string> _openMenus = new HashSet<string>();

    public SidebarStateService(ISessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public IReadOnlyCollection<string> OpenMenus => _openMenus;

    public async Task ToggleMenuAsync(string menuId)
    {
        if (_openMenus.Contains(menuId))
        {
            _openMenus.Remove(menuId);
        }
        else
        {
            _openMenus.Add(menuId);
        }

        await SaveStateAsync();
    }

    public bool IsMenuOpen(string menuId)
    {
        return _openMenus.Contains(menuId);
    }

    public async Task LoadStateAsync()
    {
        var storedState = await _sessionStorage.GetItemAsync<HashSet<string>>("SidebarState");
        if (storedState != null)
        {
            _openMenus = storedState;
        }
    }

    private async Task SaveStateAsync()
    {
        await _sessionStorage.SetItemAsync("SidebarState", _openMenus);
    }
}
