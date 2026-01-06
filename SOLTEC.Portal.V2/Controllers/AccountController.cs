using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SOLTEC.Portal.V2.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        public AccountController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiSettings = options.Value;
            _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Usuarios/Login", new
            {
                Usuario = usuario,
                Password = password
            });

            var contenido = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<JsonElement>(contenido);

            bool success = resultado.GetProperty("success").GetBoolean();

            if (success)
            {
                string nombreUsuario = resultado.GetProperty("nombreUsuario").GetString();
                int idUsuario = resultado.GetProperty("idUsuario").GetInt32();
                string empresaSucursalJson = "[]";
                string? nombreEmpresa = null;

                if (resultado.TryGetProperty("empresaSucursal", out var empresaSucursalElement) &&
                    empresaSucursalElement.ValueKind == JsonValueKind.Array &&
                    empresaSucursalElement.GetArrayLength() > 0)
                {
                    empresaSucursalJson = empresaSucursalElement.GetRawText();
                    try
                    {
                        var primeraEmpresa = empresaSucursalElement[0];
                        if (primeraEmpresa.TryGetProperty("nombreEmpresa", out var nombreEmpresaElement))
                        {
                            nombreEmpresa = nombreEmpresaElement.GetString();
                        }
                    }
                    catch
                    {
                    }
                }

                string nombreFinal = !string.IsNullOrWhiteSpace(nombreEmpresa)
                    ? nombreEmpresa
                    : nombreUsuario;

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, nombreFinal),
            new Claim("IdUsuario", idUsuario.ToString()),
            new Claim("EmpresaSucursal", empresaSucursalJson)
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                ViewBag.Error = resultado.GetProperty("mensaje").GetString();
                return View("~/Views/Home/Login.cshtml");
            }
        }




        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public IActionResult RenovarSesion()
        {
            HttpContext.Session.SetString("UltimaActividad", DateTime.Now.ToString());
            return Json(new { success = true });
        }

    }
}
