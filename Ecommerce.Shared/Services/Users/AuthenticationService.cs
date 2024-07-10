using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.Users;
public interface IAuthenticationService
{
    Task<ServiceResponse<User>> LoginAsync(LoginDto loginModel);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;

    public AuthenticationService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ServiceResponse<User>> LoginAsync(LoginDto loginModel)
    {
        return await _userService.LoginAsync(loginModel.Email, loginModel.Password);
    }
}

