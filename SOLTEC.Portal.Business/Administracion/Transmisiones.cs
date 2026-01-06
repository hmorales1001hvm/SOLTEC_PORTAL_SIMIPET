using SOLTEC.Portal.Data.Administracion;
using SOLTEC.Portal.Entities;
using SOLTEC.Portal.Entities.Administracion;
using SOLTEC.Portal.Entities.Administracion.DTOs;
using SOLTEC.Portal.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLTEC.Portal.Business.Administracion
{
    public class Transmisiones
    {
        Data.Administracion.Transmisiones transmisiones = new Data.Administracion.Transmisiones();
        public async Task<Response<ModelTransmisiones>> CargaTransmisiones(ModelTransmisiones data)
        {
            try
            {
                var result = await transmisiones.CargaTransmisiones(data);

                if (result == null)
                {
                    return new Response<ModelTransmisiones>
                    {
                        Exito = false,
                        Mensaje = "Configuración de transmisiones no encontradas.",
                    };
                }

                return new Response<ModelTransmisiones>
                {
                    Exito = true,
                    Mensaje = "Configuración de transmisiones encontradas.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelTransmisiones>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelTransmisiones>()
                };
            }
        }

        public async Task<Response<ModelTransmisiones>> CargaTransmisionesHistoricos(ModelTransmisiones data)
        {
            try
            {
                var result = await transmisiones.CargaTransmisionesHistoricos(data);

                if (result == null)
                {
                    return new Response<ModelTransmisiones>
                    {
                        Exito = false,
                        Mensaje = "Configuración de transmisiones no encontradas.",
                    };
                }

                return new Response<ModelTransmisiones>
                {
                    Exito = true,
                    Mensaje = "Configuración de transmisiones encontradas.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelTransmisiones>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelTransmisiones>()
                };
            }
        }

        public async Task<Response<ModelEmpresas>> CargaEmpresas(ModelEmpresas data)
        {
            try
            {
                var result = await transmisiones.CargaEmpresas(data);

                if (result == null)
                {
                    return new Response<ModelEmpresas>
                    {
                        Exito = false,
                        Mensaje = "Datos no encontrados."
                    };
                }

                return new Response<ModelEmpresas>
                {
                    Exito = true,
                    Mensaje = "Datos encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelEmpresas>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelEmpresas>()
                };
            }
        }


        public async Task<Response<ModelSucursales>> CargaSucursales(ModelSucursales data)
        {
            try
            {
                var result = await transmisiones.CargaSucursales(data);

                if (result == null)
                {
                    return new Response<ModelSucursales>
                    {
                        Exito = false,
                        Mensaje = "Datos no encontrados."
                    };
                }

                return new Response<ModelSucursales>
                {
                    Exito = true,
                    Mensaje = "Datos encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelSucursales>
                {
                    Exito = false,
                    Mensaje = "Error al obtener los datos",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelSucursales>()
                };
            }
        }


        public async Task<Response<ModelTiposTransmisiones>> CargaTiposTransmisiones(ModelTiposTransmisiones data)
        {
            try
            {
                var result = await transmisiones.CargaTiposTransmisiones(data);

                if (result == null)
                {
                    return new Response<ModelTiposTransmisiones>
                    {
                        Exito = false,
                        Mensaje = "Datos no encontrados."
                    };
                }

                return new Response<ModelTiposTransmisiones>
                {
                    Exito = true,
                    Mensaje = "Datos encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<ModelTiposTransmisiones>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<ModelTiposTransmisiones>()
                };
            }

        }

        public async Task<Response<ModelTransmisiones>> GuardarTransmision(ModelTransmisiones data)
        {
            return await transmisiones.GuardarTransmision(data);
        }

        public async Task<Response<ModelTransmisiones>> CancelarTransmision(string sucursal, int id)
        {
            return await transmisiones.CancelarTransmision(sucursal, id);
        }


        public async Task<Response<ModelTransmisiones>> GuardarTransmisionHistorico(ModelTransmisiones data)
        {
            return await transmisiones.GuardarTransmisionHistorico(data);
        }

        public async Task<Response<TransmisionHistorico>> SincronizaHistoricos(TransmisionHistorico data, SalesDataDto salesDataDto, string sucursal, int id)
        {
            try
            {
                var result = await transmisiones.SincronizaHistoricos(data, salesDataDto, sucursal, id);

                if (result == null)
                {
                    return new Response<TransmisionHistorico>
                    {
                        Exito = false,
                        Mensaje = "Datos no encontrados."
                    };
                }

                return new Response<TransmisionHistorico>
                {
                    Exito = true,
                    Mensaje = "Datos encontrados.",
                    DatosLista = result
                };
            }
            catch (Exception ex)
            {
                return new Response<TransmisionHistorico>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    DatosLista = new List<TransmisionHistorico>()
                };
            }
        }
    }
}
