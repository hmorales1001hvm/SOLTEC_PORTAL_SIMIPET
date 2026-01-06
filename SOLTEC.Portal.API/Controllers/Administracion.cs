using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SOLTEC.Portal.Entities;
using SOLTEC.Portal.Entities.Administracion;
using SOLTEC.Portal.Entities.Administracion.DTOs;
using SOLTEC.Portal.Entities.Seguridad;
using System.IO.Compression;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SOLTEC.Portal.API.Controllers
{
   

    [ApiController]
    [Route("api/[controller]")]
    public class AdministracionController : ControllerBase
    {
        private readonly Business.Administracion.Transmisiones _transmisiones = new Business.Administracion.Transmisiones();
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AdministracionController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpPost("Transmisiones")]
        public async Task<IActionResult> Transmisiones([FromBody] ModelTransmisiones data)
        {
            var response = await _transmisiones.CargaTransmisiones(data);
            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("TransmisionesHistoricos")]
        public async Task<IActionResult> TransmisionesHistoricos([FromBody] ModelTransmisiones data)
        {
            var response = await _transmisiones.CargaTransmisionesHistoricos(data);
            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        

        [HttpPost("Empresas")]
        public async Task<IActionResult> Empresas([FromBody] ModelEmpresas data)
        {
            var response = await _transmisiones.CargaEmpresas(data);
            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("Sucursales")]
        public async Task<IActionResult> Sucursales([FromBody] ModelSucursales data)
        {
            var response = await _transmisiones.CargaSucursales(data);

            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("TiposTransmisiones")]
        public async Task<IActionResult> TiposTransmisiones([FromBody] ModelTiposTransmisiones data)
        {
            var response = await _transmisiones.CargaTiposTransmisiones(data);

            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("GuardarTransmision")]
        public async Task<IActionResult> GuardarTransmision([FromBody] ModelTransmisiones data)
        {
            var response = await _transmisiones.GuardarTransmision(data);

            return Ok(response);
        }


        [HttpPost("GuardarTransmisionHistorico")]
        public async Task<IActionResult> GuardarTransmisionHistorico([FromBody] ModelTransmisiones data)
        {
            var response = await _transmisiones.GuardarTransmisionHistorico(data);

            return Ok(response);
        }

        
        [HttpGet("CancelarTransmision")]
        public async Task<IActionResult> CancelarTransmision(string sucursal, int id)
        {
            var response = await _transmisiones.CancelarTransmision(sucursal, id);

            return Ok(response);
        }


        [HttpGet("ProcesarHistorico")]
        public async Task<IActionResult> ProcesarHistorico(string sucursal, int id)
        {
            if (string.IsNullOrEmpty(sucursal))
                return BadRequest("Parámetro 'sucursal' requerido.");

            //var urls = _configuration.GetSection("ApiSettings:Urls").Get<string[]>();
            var urls = ConfigHelper.Configuration.GetSection("ApiSettings:Urls").Get<string[]>();

            if (urls == null || urls.Length == 0)
                return BadRequest("No hay URLs de API configuradas.");

            byte[] fileBytes = null;

            // Intentar descargar desde la primera URL disponible
            foreach (var url in urls)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    client.BaseAddress = new Uri(url.EndsWith("/") ? url : url + "/");

                    var endpoint = new Uri(client.BaseAddress, $"venta/DescargarScriptZip?sucursal={sucursal}");
                    var response = await client.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        fileBytes = await response.Content.ReadAsByteArrayAsync();
                        break;
                    }
                }
                catch
                {
                    continue;
                }
            }

            if (fileBytes == null)
                return BadRequest("No se pudo descargar el archivo desde ninguna URL activa.");

            // --- Leer ZIP ---
            using var memoryStream = new MemoryStream(fileBytes);
            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

            TransmisionHistorico transmisionHistorico = null;
            SalesDataDto salesDataDto = null;

            foreach (var entry in archive.Entries)
            {
                if (!entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    continue;

                using var entryStream = entry.Open();
                using var reader = new StreamReader(entryStream);
                string jsonContent = await reader.ReadToEndAsync();

                if (entry.Name == $"{sucursal}_infoDB.json")
                {
                    transmisionHistorico = JsonConvert.DeserializeObject<TransmisionHistorico>(jsonContent);
                }
                else if (entry.Name == $"{sucursal}_data.json")
                {
                    salesDataDto = JsonConvert.DeserializeObject<SalesDataDto>(jsonContent);
                }
            }

            if (transmisionHistorico == null || salesDataDto == null)
                return BadRequest("El ZIP no contiene los archivos JSON esperados.");

            // Procesar ambos archivos juntos
            var result = await _transmisiones.SincronizaHistoricos(transmisionHistorico, salesDataDto, sucursal, id);

            return Ok(new
            {
                Success = true,
                Message = "Archivos JSON procesados y subidos a BD correctamente.",
                Result = result
            });
        }



    }
}
