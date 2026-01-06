using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace SOLTEC.Portal.V2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {

            return View(); 
        }

        [AllowAnonymous]
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Inicio()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes; 
            ViewBag.SessionMinutes = sessionMinutes;

            ViewBag.SessionMinutes = sessionMinutes;


            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl; 
            return View(); 
        }

    }
}