using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SOLTEC.Portal.Entities.Seguridad;
using System.Security.Claims;

namespace SOLTEC.Portal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly Business.Administracion.Usuarios _usuarios = new Business.Administracion.Usuarios();

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] ModelUsuarios data)
        {
            var response = await _usuarios.Login(data);
            if (response.Exito)
                return Ok(new { success = true, mensaje = "Login correcto", nombreUsuario = response.Datos.Nombre, idUsuario = response.Datos.IdUsuario, empresaSucursal=response.DatosLista});
            else
                return Ok(new { success = false, mensaje = response.Mensaje });
        }
    }
}
