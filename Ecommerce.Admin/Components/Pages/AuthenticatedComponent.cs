using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Ecommerce.Admin.Components.Pages;

public class AuthenticatedComponent : ComponentBase
{
    [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected string UserId { get; private set; }


    
        protected override async Task OnInitializedAsync()
        {
            await InitializeUserAsync();
        }

    private async Task InitializeUserAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity.IsAuthenticated)
        {
            UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        else
        {
            UserId = null;
        }
    }

}
