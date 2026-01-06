using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SOLTEC.Portal.Entities;
using SOLTEC.Portal.Entities.Administracion;
using SOLTEC.Portal.Entities.Administracion.DTOs;
using SOLTEC.Portal.Entities.Administracion.Reportes;
using SOLTEC.Portal.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLTEC.Portal.Business.Administracion
{
    public class Reportes
    {
        Data.Administracion.Reportes reportes = new Data.Administracion.Reportes();
        public async Task<Response<ModelAcumuladoFecha>> CargaAcumuladoFecha(FiltrosAcumuladoFecha data)
        {
           
            try
            {
                var result = await reportes.CargaAcumuladoFecha(data);

                if (result == null)
                {
                    return new Response<ModelAcumuladoFecha>
                    {
                        Exito = false,
                        Mensaje = "Datos no econtrados.",
                    };
                }

                return new Response<ModelAcumuladoFecha>
                {
                    Exito = true,
                    Mensaje = "Datos no encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelAcumuladoFecha>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelAcumuladoFecha>()
                };
            }
        }


        public async Task<Response<ModelPorSucursal>> CargaPorSucursal(FiltrosPorSucursal data)
        {

            try
            {
                var result = await reportes.CargaPorSucursal(data);

                if (result == null)
                {
                    return new Response<ModelPorSucursal>
                    {
                        Exito = false,
                        Mensaje = "Datos no encontrados.",
                    };
                }

                return new Response<ModelPorSucursal>
                {
                    Exito = true,
                    Mensaje = "Datos no encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelPorSucursal>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelInventarioDetalle>()
                };
            }
        }
        

        public async Task<Response<ModelPorSucursalVendedor>> CargaPorSucursalVendedor(FiltrosPorSucursalVendedor data)
        {

            try
            {
                var result = await reportes.CargaPorSucursalVendedor(data);

                if (result == null)
                {
                    return new Response<ModelPorSucursalVendedor>
                    {
                        Exito = false,
                        Mensaje = "Datos no encontrados.",
                    };
                }

                return new Response<ModelPorSucursalVendedor>
                {
                    Exito = true,
                    Mensaje = "Datos no encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelPorSucursalVendedor>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelPorSucursalVendedor>()
                };
            }
        }

        public async Task<Response<ModelPorSucursalVendedorProductos>> CargaPorSucursalVendedorProducto(FiltrosPorSucursalVendedorProductos data)
        {

            try
            {
                var result = await reportes.CargaPorSucursalVendedorProducto(data);

                if (result == null)
                {
                    return new Response<ModelPorSucursalVendedorProductos>
                    {
                        Exito = false,
                        Mensaje = "Datos no encontrados.",
                    };
                }

                return new Response<ModelPorSucursalVendedorProductos>
                {
                    Exito = true,
                    Mensaje = "Datos no encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelPorSucursalVendedorProductos>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelPorSucursalVendedorProductos>()
                };
            }
        }

        public async Task<Response<ModelPorVendedor>> CargaPorVendedor(FiltrosPorVendedor data)
        {

            try
            {
                var result = await reportes.CargaPorVendedor(data);

                if (result == null)
                {
                    return new Response<ModelPorVendedor>
                    {
                        Exito = false,
                        Mensaje = "Datos no encontrados.",
                    };
                }

                return new Response<ModelPorVendedor>
                {
                    Exito = true,
                    Mensaje = "Datos no encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelPorVendedor>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelPorVendedor>()
                };
            }
        }


        public async Task<Response<ModelPorFechaSucursalVendedor>> CargaPorFechaSucursalVendedor(FiltrosPorFechaSucursalVendedor data)
        {

            try
            {
                var result = await reportes.CargaPorFechaSucursalVendedor(data);

                if (result == null)
                {
                    return new Response<ModelPorFechaSucursalVendedor>
                    {
                        Exito = false,
                        Mensaje = "Datos no encontrados.",
                    };
                }

                return new Response<ModelPorFechaSucursalVendedor>
                {
                    Exito = true,
                    Mensaje = "Datos no encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelPorFechaSucursalVendedor>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelPorFechaSucursalVendedor>()
                };
            }
        }


        #region INVENTARIOS
        
        public async Task<Response<ModelInventarioDetalle>> CargaInventarioDetalle(FiltrosInventarioDetalle data)
        {
            try
            {
                var result = await reportes.CargaInventarioDetalle(data);

                if (result == null)
                {
                    return new Response<ModelInventarioDetalle>
                    {
                        Exito = false,
                        Mensaje = "Datos no encontrados.",
                    };
                }

                return new Response<ModelInventarioDetalle>
                {
                    Exito = true,
                    Mensaje = "Datos no encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelInventarioDetalle>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelInventarioDetalle>()
                };
            }
        }


        public async Task<Response<ModelInventarioValuacion>> CargaInventarioValuacion(FiltrosInventarioValuacion data)
        {
            try
            {
                var result = await reportes.CargaInventarioValuacion(data);

                if (result == null)
                {
                    return new Response<ModelInventarioValuacion>
                    {
                        Exito = false,
                        Mensaje = "Datos no encontrados.",
                    };
                }

                return new Response<ModelInventarioValuacion>
                {
                    Exito = true,
                    Mensaje = "Datos no encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelInventarioValuacion>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelInventarioValuacion>()
                };
            }
        }
        
        #endregion
    }
}
