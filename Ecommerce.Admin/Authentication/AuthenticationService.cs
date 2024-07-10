using Ecommerce.Shared.Context;
using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Admin.Authentication;

public interface IAuthenticationManager
{
    Task<bool> MarkUserAsLogedIn(User user);
    Task LogoutAsync();
}

public class AuthenticationManager : IAuthenticationManager
{
    private readonly ApplicationDbContext _context;
    private readonly CustomAuthenticationStateProvider _authenticationStateProvider;

    public AuthenticationManager(ApplicationDbContext context, CustomAuthenticationStateProvider authenticationStateProvider)
    {
        _context = context;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<bool> MarkUserAsLogedIn(User user)
    {
       
           await _authenticationStateProvider.MarkUserAsAuthenticated(user);
            return true;
       
    }

    public Task LogoutAsync()
    {
        _authenticationStateProvider.MarkUserAsLoggedOut();
        return Task.CompletedTask;
    }



}
