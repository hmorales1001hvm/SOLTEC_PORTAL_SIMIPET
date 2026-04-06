using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO.Compression;

namespace SOLTEC.Portal.V2.Controllers
{
    
    public class AdministracionController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApiUrlResolver _apiUrlResolver;
        private readonly IHttpClientFactory _httpClientFactory;

        public AdministracionController(IConfiguration configuration, IHttpClientFactory httpClientFactory,  ApiUrlResolver apiUrlResolver)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _apiUrlResolver = apiUrlResolver;
        }

        public IActionResult ConfiguradorTransmision()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes; // 60
            ViewBag.SessionMinutes = sessionMinutes;

            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl;
            return View();
        }

        public IActionResult ConfiguradorHistoricos()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes; // 60
            ViewBag.SessionMinutes = sessionMinutes;

            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl;
            return View();
        }

        public IActionResult AcumuladoFecha()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes; 
            ViewBag.SessionMinutes = sessionMinutes;

            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl;
            return View();
        }

        public IActionResult PorFechaSucursalVendedor()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes;
            ViewBag.SessionMinutes = sessionMinutes;

            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl;
            return View();
        }

        public IActionResult PorSucursal()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes;
            ViewBag.SessionMinutes = sessionMinutes;

            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl;
            return View();
        }

        public IActionResult PorSucursalVendedor()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes;
            ViewBag.SessionMinutes = sessionMinutes;

            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl;
            return View();
        }

        public IActionResult PorVendedor()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes;
            ViewBag.SessionMinutes = sessionMinutes;

            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl;
            return View();
        }

        public IActionResult InventarioDetalle()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes;
            ViewBag.SessionMinutes = sessionMinutes;

            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl;
            return View();
        }


        public IActionResult InventarioValuacion()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes;
            ViewBag.SessionMinutes = sessionMinutes;

            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl;
            return View();
        }

        public IActionResult PorSucursalVendedorProductos()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes;
            ViewBag.SessionMinutes = sessionMinutes;

            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl;
            return View();
        }

        public IActionResult UsuariosSIMIPET()
        {
            var monitor = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var options = monitor.Get(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionMinutes = (int)options.ExpireTimeSpan.TotalMinutes;
            ViewBag.SessionMinutes = sessionMinutes;

            string apiUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.ApiBaseUrl = apiUrl;
            return View();
        }
    }
}
