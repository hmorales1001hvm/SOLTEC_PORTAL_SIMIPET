using Microsoft.AspNetCore.Mvc;
using SOLTEC.Portal.Entities.Administracion.DTOs;

namespace SOLTEC.Portal.API.Controllers
{
   

    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly Business.Administracion.Reportes reportes = new Business.Administracion.Reportes();

        [HttpPost("GetAcumuladoFecha")]
        public async Task<IActionResult> GetAcumuladoFecha([FromBody] FiltrosAcumuladoFecha data)
        {
            var response = await reportes.CargaAcumuladoFecha(data);
            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("GetPorSucursal")]
        public async Task<IActionResult> GetPorSucursal([FromBody] FiltrosPorSucursal data)
        {
            var response = await reportes.CargaPorSucursal(data);
            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("GetPorSucursalVendedor")]
        public async Task<IActionResult> GetPorSucursalVendedor([FromBody] FiltrosPorSucursalVendedor data)
        {
            var response = await reportes.CargaPorSucursalVendedor(data);
            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("GetPorSucursalVendedorProducto")]
        public async Task<IActionResult> GetPorSucursalVendedorProducto([FromBody] FiltrosPorSucursalVendedorProductos data)
        {
            var response = await reportes.CargaPorSucursalVendedorProducto(data);
            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("GetPorVendedor")]
        public async Task<IActionResult> GetPorVendedor([FromBody] FiltrosPorVendedor data)
        {
            var response = await reportes.CargaPorVendedor(data);
            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("GetPorFechaSucursalVendedor")]
        public async Task<IActionResult> GetPorFechaSucursalVendedor([FromBody] FiltrosPorFechaSucursalVendedor data)
        {
            var response = await reportes.CargaPorFechaSucursalVendedor(data);
            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        #region INVENTARIOS
        [HttpPost("GetInventarioDetalle")]
        public async Task<IActionResult> GetInventarioDetalle([FromBody] FiltrosInventarioDetalle data)
        {
            var response = await reportes.CargaInventarioDetalle(data);
            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("GetInventarioValuacion")]
        public async Task<IActionResult> GetInventarioValuacion([FromBody] FiltrosInventarioValuacion data)
        {
            var response = await reportes.CargaInventarioValuacion(data);
            if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }
        #endregion

    }



}
