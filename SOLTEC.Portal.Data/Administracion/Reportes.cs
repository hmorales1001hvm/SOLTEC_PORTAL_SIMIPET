using Dapper;
using Microsoft.Data.SqlClient;
using SOLTEC.Portal.Entities.Administracion.DTOs;
using SOLTEC.Portal.Entities.Administracion.Reportes;
using System.Data;
using static Dapper.SqlMapper;
namespace SOLTEC.Portal.Data.Administracion
{
    public class Reportes
    {
        static string connectionStringSQL = Settings1.Default.DbSimiPET_SQLSERVER;

        public async Task<List<ModelAcumuladoFecha>> CargaAcumuladoFecha(FiltrosAcumuladoFecha data)
        {
            using (var connection = new SqlConnection(connectionStringSQL))
            {
                var parameter = new DynamicParameters();

                parameter.Add("@Desde", data.Desde.ToString("yyyyMMdd"), DbType.Int32);
                parameter.Add("@Hasta", data.Hasta.ToString("yyyyMMdd"), DbType.Int32);

                
                var tvp = new DataTable();
                tvp.Columns.Add("ClavesSimi", typeof(string));

                if (data.ClavesSIMI != null && data.ClavesSIMI.Any())
                {
                    foreach (var clave in data.ClavesSIMI)
                        tvp.Rows.Add(clave);
                }

                parameter.Add("@ClavesSimi", tvp.AsTableValuedParameter("dbo.ClaveSimiList"));

                await connection.OpenAsync();

                var result = await connection.QueryAsync<ModelAcumuladoFecha>(
                    "usp_ReportesCargaAcumuladoFecha",
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 420
                );

                return result.ToList();
            }
        }


        public async Task<List<ModelPorSucursal>> CargaPorSucursal(FiltrosPorSucursal data)
        {
            var _data = new List<ModelPorSucursal>();

            using (var connection = new SqlConnection(connectionStringSQL))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Desde", data.Desde.ToString("yyyyMMdd"), DbType.Int32);
                parameter.Add("@Hasta", data.Hasta.ToString("yyyyMMdd"), DbType.Int32);
                var tvp = new DataTable();
                tvp.Columns.Add("ClavesSimi", typeof(string));

                if (data.ClavesSIMI != null && data.ClavesSIMI.Any())
                {
                    foreach (var clave in data.ClavesSIMI)
                        tvp.Rows.Add(clave);
                }

                parameter.Add("@ClavesSimi", tvp.AsTableValuedParameter("dbo.ClaveSimiList"));


                await connection.OpenAsync();

                _data = (await connection.QueryAsync<ModelPorSucursal>(
                    "usp_ReportesCargaPorSucursal",
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 420
                )).ToList();

                await connection.CloseAsync();
            }

            return _data;
        }

        public async Task<List<ModelPorSucursalVendedor>> CargaPorSucursalVendedor(FiltrosPorSucursalVendedor data)
        {
            var _data = new List<ModelPorSucursalVendedor>();

            using (var connection = new SqlConnection(connectionStringSQL))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Desde", data.Desde.ToString("yyyyMMdd"), DbType.Int32);
                parameter.Add("@Hasta", data.Hasta.ToString("yyyyMMdd"), DbType.Int32);
                var tvp = new DataTable();
                tvp.Columns.Add("ClavesSimi", typeof(string));

                if (data.ClavesSIMI != null && data.ClavesSIMI.Any())
                {
                    foreach (var clave in data.ClavesSIMI)
                        tvp.Rows.Add(clave);
                }

                parameter.Add("@ClavesSimi", tvp.AsTableValuedParameter("dbo.ClaveSimiList"));


                await connection.OpenAsync();

                _data = (await connection.QueryAsync<ModelPorSucursalVendedor>(
                    "usp_ReportesCargaPorSucursalVendedor",
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 420
                )).ToList();

                await connection.CloseAsync();
            }

            return _data;
        }


        public async Task<List<ModelPorSucursalVendedorProductos>> CargaPorSucursalVendedorProducto(FiltrosPorSucursalVendedorProductos data)
        {
            var _data = new List<ModelPorSucursalVendedorProductos>();

            using (var connection = new SqlConnection(connectionStringSQL))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Desde", data.Desde.ToString("yyyyMMdd"), DbType.Int32);
                parameter.Add("@Hasta", data.Hasta.ToString("yyyyMMdd"), DbType.Int32);
                var tvp = new DataTable();
                tvp.Columns.Add("ClavesSimi", typeof(string));

                if (data.ClavesSIMI != null && data.ClavesSIMI.Any())
                {
                    foreach (var clave in data.ClavesSIMI)
                        tvp.Rows.Add(clave);
                }

                parameter.Add("@ClavesSimi", tvp.AsTableValuedParameter("dbo.ClaveSimiList"));


                await connection.OpenAsync();

                _data = (await connection.QueryAsync<ModelPorSucursalVendedorProductos>(
                    "usp_ReportesCargaProductosPorFechaSucursalVendedor",
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 420
                )).ToList();

                await connection.CloseAsync();
            }

            return _data;
        }

        public async Task<List<ModelPorVendedor>> CargaPorVendedor(FiltrosPorVendedor data)
        {
            var _data = new List<ModelPorVendedor>();

            using (var connection = new SqlConnection(connectionStringSQL))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Desde", data.Desde.ToString("yyyyMMdd"), DbType.Int32);
                parameter.Add("@Hasta", data.Hasta.ToString("yyyyMMdd"), DbType.Int32);
                var tvp = new DataTable();
                tvp.Columns.Add("ClavesSimi", typeof(string));

                if (data.ClavesSIMI != null && data.ClavesSIMI.Any())
                {
                    foreach (var clave in data.ClavesSIMI)
                        tvp.Rows.Add(clave);
                }

                parameter.Add("@ClavesSimi", tvp.AsTableValuedParameter("dbo.ClaveSimiList"));


                await connection.OpenAsync();

                _data = (await connection.QueryAsync<ModelPorVendedor>(
                    "usp_ReportesCargaPorVendedor",
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 420
                )).ToList();

                await connection.CloseAsync();
            }

            return _data;
        }

        public async Task<List<ModelPorFechaSucursalVendedor>> CargaPorFechaSucursalVendedor(FiltrosPorFechaSucursalVendedor data)
        {
            var _data = new List<ModelPorFechaSucursalVendedor>();

            using (var connection = new SqlConnection(connectionStringSQL))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Desde", data.Desde.ToString("yyyyMMdd"), DbType.Int32);
                parameter.Add("@Hasta", data.Hasta.ToString("yyyyMMdd"), DbType.Int32);
                var tvp = new DataTable();
                tvp.Columns.Add("ClavesSimi", typeof(string));

                if (data.ClavesSIMI != null && data.ClavesSIMI.Any())
                {
                    foreach (var clave in data.ClavesSIMI)
                        tvp.Rows.Add(clave);
                }

                parameter.Add("@ClavesSimi", tvp.AsTableValuedParameter("dbo.ClaveSimiList"));


                await connection.OpenAsync();

                _data = (await connection.QueryAsync<ModelPorFechaSucursalVendedor>(
                    "usp_ReportesCargaPorFechaSucursalVendedor",
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 420
                )).ToList();

                await connection.CloseAsync();
            }

            return _data;
        }

        #region INVENTARIOS
        public async Task<List<ModelInventarioDetalle>> CargaInventarioDetalle(FiltrosInventarioDetalle data)
        {
            var _data = new List<ModelInventarioDetalle>();

            using (var connection = new SqlConnection(connectionStringSQL))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Desde", data.Desde.ToString("yyyyMMdd"), DbType.Int32);
                //parameter.Add("@Hasta", data.Hasta.ToString("yyyyMMdd"), DbType.Int32);
                var tvp = new DataTable();
                tvp.Columns.Add("ClavesSimi", typeof(string));

                if (data.ClavesSIMI != null && data.ClavesSIMI.Any())
                {
                    foreach (var clave in data.ClavesSIMI)
                        tvp.Rows.Add(clave);
                }

                parameter.Add("@ClavesSimi", tvp.AsTableValuedParameter("dbo.ClaveSimiList"));


                await connection.OpenAsync();

                _data = (await connection.QueryAsync<ModelInventarioDetalle>(
                    "usp_ReportesCargaInventarioDetalle",
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 420
                )).ToList();

                await connection.CloseAsync();
            }

            return _data;
        }

        public async Task<List<ModelInventarioValuacion>> CargaInventarioValuacion(FiltrosInventarioValuacion data)
        {
            var _data = new List<ModelInventarioValuacion>();

            using (var connection = new SqlConnection(connectionStringSQL))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Desde", data.Desde.ToString("yyyyMMdd"), DbType.Int32);
                //parameter.Add("@Hasta", data.Hasta.ToString("yyyyMMdd"), DbType.Int32);
                var tvp = new DataTable();
                tvp.Columns.Add("ClavesSimi", typeof(string));

                if (data.ClavesSIMI != null && data.ClavesSIMI.Any())
                {
                    foreach (var clave in data.ClavesSIMI)
                        tvp.Rows.Add(clave);
                }

                parameter.Add("@ClavesSimi", tvp.AsTableValuedParameter("dbo.ClaveSimiList"));


                await connection.OpenAsync();

                _data = (await connection.QueryAsync<ModelInventarioValuacion>(
                    "usp_ReportesCargaInventarioValuacion",
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 420
                )).ToList();

                await connection.CloseAsync();
            }

            return _data;
        }
        #endregion
    }
}
