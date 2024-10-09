using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using OfficeOpenXml;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Admin.Components.Pages;

public class AuthenticatedComponent : ComponentBase
{
    [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected long UserId { get; private set; }

    protected bool IsAgent { get; private set; }

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
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (long.TryParse(userIdClaim, out var parsedUserId))
            {
                UserId = parsedUserId;
            }
            else
            {
                UserId = 0; 
            }



            var isAgentClaim = user.FindFirst(ClaimTypes.Actor)?.Value;
            IsAgent = isAgentClaim == "Agent";
        }
        else
        {
            UserId = 0 ;
            IsAgent = false;
        }
    }

}





