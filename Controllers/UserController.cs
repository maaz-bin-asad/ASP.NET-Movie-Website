using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using DotNetAssignment.Models;
using DotNetAssignment.Services;

namespace DotNetAssignment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

       [HttpGet("checkAuth")]
       public bool CheckAuthentication()
        {
            Console.WriteLine(HttpContext.Request.Cookies.ContainsKey("AuthCookie"));
            if (HttpContext.Request.Cookies.ContainsKey("AuthCookie")) return true;
            return false;

        }

        [HttpPost("login")]
        public LocalRedirectResult LoginUser([FromForm] User user)
        {
            
            if (UserServices.LoginUser(user))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.mail)
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Login");

                HttpContext.Session.SetString("mail",user.mail);
                Console.WriteLine(HttpContext.Session.GetString("mail"));  
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return LocalRedirect("/");;
            }
            return LocalRedirect("/login?invalid=1");
            
        }

        [HttpGet("logout")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);  
            return Redirect("/login");
        }

        [HttpPost("signup")]
        public LocalRedirectResult RegisterUser([FromForm] User user) 
        {
            if (UserServices.RegisterUser(user))
            {
               return LocalRedirect("/login?registered=1");
            }
           return LocalRedirect("/signup?invalid=1");
           
        }
        /*
        [HttpGet("getUser")]

        public async Task<User>GetUser([FromQuery] string username)
        {

            return await UserServices.GetUser(username);
        }
        */
    }
}
