using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using static Dapper.SqlMapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SOLTEC.Portal.Entities.Seguridad;
using Dapper;
using SOLTEC.Portal.Entities.Administracion;
using System.Globalization;
using SOLTEC.Portal.Entities;
namespace SOLTEC.Portal.Data.Administracion
{
    public class Usuarios
    {
        static string conectionStringFacturacion = Settings1.Default.ConectionStringFactura;
        static string conextionStringFacturaRealOrquestador = Settings1.Default.ConectionStringFacturaOrquestador;


        public async Task<ModelUsuarios> Login(ModelUsuarios data)
        {
            var _data = new ModelUsuarios();

            using (var connection = new MySqlConnection(conextionStringFacturaRealOrquestador))
            {
                try
                {
                    await connection.OpenAsync();

                    // 1. Obtener usuario
                    var query = @" SELECT IdUsuario,
                                  Nombre,
                                  Usuario, 
                                  Contrasena AS Password,
                                  IFNULL(IdEmpresa,0) AS IdEmpresa 
                           FROM soltec2_PortalUsuarios 
                           WHERE Usuario = @Usuario AND Contrasena = @Password
                           LIMIT 1;";

                    _data = await connection.QueryFirstOrDefaultAsync<ModelUsuarios>(
                        query,
                        new { Usuario = data.Usuario, Password = data.Password },
                        commandTimeout: 420
                    );

                    if (_data == null)
                        return null;

                    // 2. Obtener lista de empresa–sucursal
                    if (_data.IdEmpresa > 0)
                    {
                        var sql = @"SELECT  
                                A.nombre_empresa AS NombreEmpresa, 
                                B.claveSimi AS ClaveSimi, 
                                B.nombre AS NombreSucursal
                            FROM SimiPET.catempresa A 
                            INNER JOIN SimiPET.sucursal B 
                                ON A.idEmpresa = B.idEmpresa 
                            WHERE A.idEmpresa = @IdEmpresa";

                        var lista = await connection.QueryAsync<ModelEmpresaSucursal>(
                            sql,
                            new { IdEmpresa = _data.IdEmpresa },
                            commandTimeout: 420
                        );

                        // Convertir a lista en caso de que Dapper regrese IEnumerable
                        _data.EmpresaSucursal = lista.ToList();
                    }
                    else
                        _data.EmpresaSucursal = new List<ModelEmpresaSucursal>();
                 }
                catch
                {
                    return null;
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            return _data;
        }


    }
}
