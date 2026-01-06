using SOLTEC.Portal.Entities;
using SOLTEC.Portal.Entities.Administracion;
using SOLTEC.Portal.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLTEC.Portal.Business.Administracion
{
    public class Usuarios
    {
        Data.Administracion.Usuarios usuarios = new Data.Administracion.Usuarios();
        public async Task<Response<ModelUsuarios>> Login(ModelUsuarios data)
        {
            try
            {
                var result = await usuarios.Login(data);
                if (result == null)
                {
                    return new Response<ModelUsuarios>
                    {
                        Exito = false,
                        Mensaje = "Error al obtener los datos.",
                    };
                }
                else
                {
                    if (result.Usuario != null)
                    {
                        return new Response<ModelUsuarios>
                        {
                            Exito = true,
                            Mensaje = "Usuario encontrado",
                            Datos = result,
                            DatosLista = result.EmpresaSucursal.ToList()
                            
                        };
                    } else
                    {
                        return new Response<ModelUsuarios>
                        {
                            Exito = false,
                            Mensaje = "Usuario o contraseña incorrectos.",
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<ModelUsuarios>
                {
                    Exito = false,
                    Mensaje = $"Error al obtener los datos, error: {ex.Message}",
                    CodigoError = "ERROR",
                    Datos = new ModelUsuarios()
                };
            }
        }
        
    }
}
