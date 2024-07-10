using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Services.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Admin.Authentication
{
    public class AuthController : Controller
    {
        /// <summary>
        /// changes
        /// </summary>

        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("/Auth/CookieLogin")]
        public async Task<IActionResult> CookieLogin(IFormCollection collection)
        {
         

            //Generate the claims
            string email = collection["Email"];
            string password = collection["Password"];


            var result = await _userService.LoginAsync(email, password);
            if (result.Success)
            {
                User user = result.Data;

                var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Role, user.Role.Name) ,
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claim, "Auth"));
                await HttpContext.SignInAsync("Auth", principal);

                return Redirect("/");
            }
            else
            {

                var path = "/login?error=" + result.Message;
                return Redirect(path);
            }


            ////var claims = new List<Claim>();
            ////claims.Add(new Claim(ClaimTypes.Name, Email));
            ////claims.Add(new Claim(ClaimTypes.Role, Passwors));

            //var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Auth"));

            //await HttpContext.SignInAsync("Auth", principal).ConfigureAwait(false);

            //return Redirect("/");
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("/Auth/Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginModel)
        {

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, loginModel.Email));
            claims.Add(new Claim(ClaimTypes.Role, loginModel.Password));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Auth"));

            await HttpContext.SignInAsync("Auth", principal).ConfigureAwait(false);

            return Redirect("/");





        }

    }
}
