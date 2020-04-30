using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LiveChat.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace LiveChat.Controllers
{

    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            if (loginViewModel.UserName == "admin" && loginViewModel.Password == "12345")
            {
                var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name,loginViewModel.UserName),
                        new Claim(ClaimTypes.Role,"SupportAgent")
                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.
                    AuthenticationScheme);

                var properties = new AuthenticationProperties
                {
                    RedirectUri = Url.Content("/Support")
                };
                return SignIn(new ClaimsPrincipal(identity), properties,
                    CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return View(loginViewModel);
        }


    }
}