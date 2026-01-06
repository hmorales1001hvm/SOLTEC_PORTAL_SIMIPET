using Dapper;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using SOLTEC.Portal.Entities;
using SOLTEC.Portal.Entities.Administracion;
using SOLTEC.Portal.Entities.Administracion.DTOs;
using SOLTEC.Portal.Entities.Seguridad;
using System.Data;
using static Dapper.SqlMapper;
namespace SOLTEC.Portal.Data.Administracion
{


    public class Transmisiones
    {
        static string conectionStringFacturacion = Settings1.Default.ConectionStringFactura;
        static string conextionStringFacturaRealOrquestador = Settings1.Default.ConectionStringFacturaOrquestador;
        private int batchSize = 1500;
        public async Task<List<ModelTransmisiones>> CargaTransmisiones(ModelTransmisiones data)
        {

            var _data = new List<ModelTransmisiones>();

            using (var connection = new MySqlConnection(conextionStringFacturaRealOrquestador))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@pIdEmpresa", data.IdEmpresa, DbType.Int32);
                parameter.Add("@pIdSQLScript", data.IdSQLScript, DbType.Int32);
                parameter.Add("@pIdUsuario", data.IdUsuario, DbType.Int32);

                connection.Open();
                _data = (await connection.QueryAsync<ModelTransmisiones>("usp_PortalCargaTransmisionesUsuarios", parameter, commandType: CommandType.StoredProcedure, commandTimeout: 420)).ToList();
                connection.Close();

            }
            return _data;
        }


        public async Task<List<ModelTransmisiones>> CargaTransmisionesHistoricos(ModelTransmisiones data)
        {

            var _data = new List<ModelTransmisiones>();

            using (var connection = new MySqlConnection(conextionStringFacturaRealOrquestador))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@pIdEmpresa", data.IdEmpresa, DbType.Int32);
                parameter.Add("@pIdUsuario", data.IdUsuario, DbType.Int32);
                parameter.Add("@pHistorico", data.TipoCarga, DbType.Int32);
                parameter.Add("@pEstatus", data.Estatus, DbType.String);


                await connection.OpenAsync();
                _data = (await connection.QueryAsync<ModelTransmisiones>("usp_PortalCargaTransmisionesHistoricos", parameter, commandType: CommandType.StoredProcedure, commandTimeout: 420)).ToList();
                await connection.CloseAsync();

            }
            return _data;
        }

        
        public async Task<List<ModelEmpresas>> CargaEmpresas(ModelEmpresas data)
        {

            var _data = new List<ModelEmpresas>();

            using (var connection = new MySqlConnection(conextionStringFacturaRealOrquestador))
            {
                connection.Open();
                _data = (await connection.QueryAsync<ModelEmpresas>("SELECT idEmpresa IdEmpresa, nombre_empresa Empresa FROM catempresa",
                            commandType: CommandType.Text,
                            commandTimeout: 420)).ToList();
                connection.Close();

            }
            return _data;
        }

        public async Task<List<ModelSucursales>> CargaSucursales(ModelSucursales data)
        {
            using (var connection = new MySqlConnection(conextionStringFacturaRealOrquestador))
            {
                string sql = string.Empty;
                if (string.IsNullOrEmpty(data.LlaveUnica))
                {
                    sql = @"SELECT  
                                s.IdSucursal, 
                                s.claveSimi AS Clave, 
                                s.nombre AS Sucursal,
                                s.idEmpresa,
                                IF(B.NumeroSucursal IS NULL, 0, 1) AS Seleccionada,
                                B.IdUsuario
                            FROM sucursal s
                            LEFT JOIN spos_sqlscriptsdetalle B ON s.claveSimi = B.NumeroSucursal
                            WHERE s.idEmpresa = @IdEmpresa AND B.NumeroSucursal IS NULL AND s.Estatus ='A'  
                            ORDER BY CONCAT(s.claveSimi, ' - ', s.nombre);";
                } else
                {
                    sql = @"SELECT *, 1 Seleccionada 
                                    FROM (
                                        SELECT
                                            *,
                                            MD5(CONCAT_WS('|',
                                                IdSQLScript,
                                                Empresa,
                                                Tipo,
                                                TransmitirDesde,
                                                TransmitirHasta,
                                                NombreUsuario
                                            )) AS LlaveUnica
                                        FROM (
                                            SELECT 
                                                A.IdSQLScript,
                                                B.claveSimi AS Clave,
                                                B.Nombre AS Sucursal,
                                                C.nombre_empresa AS Empresa, 
                                                CASE 
                                                    WHEN D.Tipo = 'DTS' THEN 'SET DE TICKETS'
                                                    WHEN D.Tipo = 'SVL' THEN 'SET DE VENTAS'
                                                    WHEN D.Tipo = 'SQS|PLANO' THEN 'SQS - INVENTARIOS'
                                                    ELSE D.Tipo
                                                END AS Tipo, 
                                                CONCAT(DATE_FORMAT(A.Desde, '%d-%m-%Y'), ' - ', DATE_FORMAT(A.Hasta, '%d-%m-%Y')) AS Vigencia, 
                                                CONCAT(
                                                    SUBSTRING_INDEX(SUBSTRING_INDEX(A.Param1, '''', 2),'''',-1), 
                                                    ' - ', 
                                                    SUBSTRING_INDEX(SUBSTRING_INDEX(A.Param2, '''', 2),'''',-1)
                                                ) AS PeriodoTransmision, 
                                                A.Activo, 
                                                SUBSTRING_INDEX(SUBSTRING_INDEX(A.Param1, '''', 2),'''',-1) AS TransmitirDesde,
                                                SUBSTRING_INDEX(SUBSTRING_INDEX(A.Param2, '''', 2),'''',-1) AS TransmitirHasta,
                                                E.Nombre AS NombreUsuario
                                            FROM spos_sqlscriptsdetalle A 
                                            INNER JOIN sucursal B ON A.NumeroSucursal = B.claveSimi AND B.Estatus ='A'
                                            INNER JOIN catempresa C ON B.idEmpresa = C.idEmpresa
                                            INNER JOIN spos_sqlscripts D ON A.IdSQLScript = D.IdSQLScript AND D.Activo = 1
                                            LEFT JOIN soltec2_PortalUsuarios E ON A.IdUsuario = E.IdUsuario
                                        ) AS TMP
                                    ) AS FINAL
                                    WHERE FINAL.LlaveUnica = @LlaveUnica";
                }

                var _data = (await connection.QueryAsync<ModelSucursales>(
                    sql,
                    new {data.IdEmpresa,  data.LlaveUnica },  // <- pasar IdUsuario
                    commandType: CommandType.Text,
                    commandTimeout: 420
                )).ToList();

                return _data;
            }
        }



        public async Task<List<ModelTiposTransmisiones>> CargaTiposTransmisiones(ModelTiposTransmisiones data)
        {

            var _data = new List<ModelTiposTransmisiones>();

            using (var connection = new MySqlConnection(conextionStringFacturaRealOrquestador))
            {
                var sql = @"SELECT 
                                    D.IdSQLScript, 
                                    D.Tipo IdTipoTransmision,
                                    CASE 
                                        WHEN D.Tipo = 'DTS' THEN 'SET DE TICKETS'
                                        WHEN D.Tipo = 'SVL' THEN 'SET DE VENTAS'
                                        WHEN D.Tipo = 'SQS|PLANO' THEN 'SQS - INVENTARIOS'
                                        ELSE D.Tipo
                                    END AS TipoTransmision
                                FROM spos_sqlscripts D
                                WHERE D.Activo = 1;";
                connection.Open();
                _data = (await connection.QueryAsync<ModelTiposTransmisiones>(sql, commandType: CommandType.Text, commandTimeout: 420)).ToList();
                connection.Close();

            }
            return _data;
        }

        public async Task<Response<ModelTransmisiones>> GuardarTransmision(ModelTransmisiones data)
        {
            using (var connection = new MySqlConnection(conextionStringFacturaRealOrquestador))
            {
                try
                {
                    await connection.OpenAsync();
                    foreach (var item in data.ListaSucursales)
                    {
                        if (item.Clave.Trim() != "")
                        {

                            if (item.Seleccionada)
                            {
                                string sqlValidar = @"SELECT COUNT(*) FROM spos_sqlscriptsdetalle 
                                                      WHERE NumeroSucursal = @Clave AND IdSQLScript = @IdSQLScript";
                                var existe = await connection.ExecuteScalarAsync<int>(sqlValidar, new { item.Clave, data.IdSQLScript });

                                var param1 = $@"DATE_FORMAT('{data.TransmitirDesde}','%Y%m%d')";
                                var param2 = $@"DATE_FORMAT('{data.TransmitirHasta}','%Y%m%d')";

                                string sql;

                                if (existe>0)
                                {
                                    // Actualización
                                    sql = @"UPDATE spos_sqlscriptsdetalle 
                                            SET Dias = @Dias,
                                                Desde = SYSDATE(),
                                                Hasta = SYSDATE(),
                                                Param1 = @Param1,
                                                Param2 = @Param2,        
                                                Activo = @Activo,
                                                IdUsuario = @IdUsuario
                                            WHERE NumeroSucursal = @Clave AND IdSQLScript = @IdSQLScript;";
                                }
                                else
                                {
                                    // Inserción
                                    sql = @"INSERT INTO spos_sqlscriptsdetalle 
                                            (NumeroSucursal, Dias, Desde, Hasta, Activo, IdSQLScript, Param1, Param2, IdUsuario)
                                            VALUES 
                                            (@Clave, @Dias, SYSDATE(), SYSDATE(), @Activo, @IdSQLScript, @Param1, @Param2, @IdUsuario);";
                                                    }

                                await connection.ExecuteAsync(sql, new
                                {
                                    data.Id,
                                    item.Clave,
                                    data.Dias,
                                    Param1 = param1,
                                    Param2 = param2,
                                    data.Activo,
                                    data.IdSQLScript,
                                    data.IdUsuario
                                });
                            } else
                            {
                                // Eliminar si no está seleccionada
                                string sqlEliminar = @"DELETE FROM spos_sqlscriptsdetalle 
                                                       WHERE NumeroSucursal = @Clave AND IdSQLScript = @IdSQLScript;";
                                await connection.ExecuteAsync(sqlEliminar, new { item.Clave, data.IdSQLScript });
                            }
                        
                        }
                    }
                    return new Response<ModelTransmisiones> { Exito = true, Mensaje = "Transmisión guardada correctamente." };
                }
                catch (Exception ex)
                {
                    return new Response<ModelTransmisiones> { Exito = false, Mensaje = ex.Message };
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }


        public async Task<Response<ModelTransmisiones>> CancelarTransmision(string sucursal, int id)
        {
            using (var connection = new MySqlConnection(conextionStringFacturaRealOrquestador))
            {
                try
                {
                    var sql = string.Empty;
                    await connection.OpenAsync();
                    
                    sql = @"UPDATE soltec2_Historicos
                            SET Activo = 0, Estatus ='ELIMINADO'
                            WHERE ClaveSimi = @sucursal AND IdHistorico = @id;";

                    await connection.ExecuteAsync(sql, new
                    {
                        sucursal = sucursal,
                        id = id
                    });

                    return new Response<ModelTransmisiones> { Exito = true, Mensaje = "Transmisión guardada correctamente." };
                }
                catch (Exception ex)
                {
                    return new Response<ModelTransmisiones> { Exito = false, Mensaje = ex.Message };
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<Response<ModelTransmisiones>> GuardarTransmisionHistorico(ModelTransmisiones data)
        {
            using (var connection = new MySqlConnection(conextionStringFacturaRealOrquestador))
            {
                try
                {
                    await connection.OpenAsync();
                    foreach (var item in data.ListaSucursales)
                    {
                        int esHistorico = (data.TipoCarga == 2) ? 1 : 0;
                        if (item.Clave.Trim() != "")
                        {
                            if (item.Seleccionada)
                            {
                                string sqlValidar = @"  SELECT COUNT(*) FROM soltec2_Historicos WHERE ClaveSimi = @Clave AND Activo = 1 
                                                          AND Estatus = 'PENDIENTE'
                                                          AND EsHistorico = @EsHistorico;
";
                                var existe = await connection.ExecuteScalarAsync<int>(sqlValidar, new { item.Clave, EsHistorico = esHistorico });

                                var param1 = data.TransmitirDesde.Replace("-","").Replace("-","");
                                var param2 = data.TransmitirHasta.Replace("-", "").Replace("-", ""); ;

                                string sql = string.Empty;

                                if (existe <= 0)
                                {
                                    // Inserción
                                    string sqlInsert = @"
                                                        INSERT INTO soltec2_Historicos 
                                                        (IdSQLScript,
                                                         Desde,
                                                         Hasta,
                                                         IdUsuario,
                                                         Estatus,
                                                         Activo,
                                                         EsHistorico,
                                                         ClaveSimi,
                                                         FechaCreacion)
                                                        VALUES 
                                                        (88, @Param1, @Param2, @IdUsuario, 'PENDIENTE', 1, @EsHistorico, @Clave, SYSDATE());
                                                    ";

                                    await connection.ExecuteAsync(sqlInsert, new
                                    {
                                        Param1 = param1,
                                        Param2 = param2,
                                        data.IdUsuario,
                                        item.Clave,
                                        EsHistorico = esHistorico
                                    });
                                }
                            }
                        }
                    }
                    return new Response<ModelTransmisiones> { Exito = true, Mensaje = "Transmisión guardada correctamente." };
                }
                catch (Exception ex)
                {
                    return new Response<ModelTransmisiones> { Exito = false, Mensaje = ex.Message };
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }


        public async Task<Response<TransmisionHistorico>> SincronizaHistoricos(TransmisionHistorico data, SalesDataDto salesDataDto, string sucursal, int id)
        {
            string connString = $"Server={data.HostName};Database={data.DatabaseName};User Id={data.UserName};Password={data.Password};TrustServerCertificate=True;Connect Timeout=60;;Max Pool Size=300;";

            using var connection = new SqlConnection(connString);
            var nombreProceso = string.Empty;

            try
            {
                using (var conn = new MySqlConnection(conextionStringFacturaRealOrquestador))
                {
                    conn.Open();
                    string query = @"UPDATE soltec2_Historicos SET Estatus = 'PROCESANDO...' WHERE ClaveSimi = @ClaveSimi AND IdHistorico=@IdHistorico";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClaveSimi", sucursal);
                        cmd.Parameters.AddWithValue("@IdHistorico", id);
                        cmd.ExecuteNonQuery();
                    }


                    conn.Close();
                }

               connection.Open();
               Console.WriteLine($"Iniciando SincronizaSetDeTransmisionesSQLServer - Sucursal: {sucursal}");

                // Deserializar DTO raíz
                var dto = salesDataDto; //JsonSerializer.Deserialize<SalesDataDto>(dataJSON) ?? new SalesDataDto();

                // 1) Ventas
                var ventasValidas = dto.Ventas?.Where(v => v.Id_Venta != null).ToList();
                nombreProceso = "Ventas";
               Console.WriteLine($"Procesando Ventas ({ventasValidas?.Count ?? 0})...");
                await BulkMergeAsync(connection, ventasValidas, @"
                CREATE TABLE #TempVentas (
                    FechaOperacion DATETIME NOT NULL,
                    ClaveSimi CHAR(10) NOT NULL,
                    Id_Venta INT NOT NULL,
                    id_usuario_venta VARCHAR(50) NOT NULL,
                    Empleado VARCHAR(100) NOT NULL,
                    idRegistradora INT NOT NULL,
                    idRegistradoraVenta INT NOT NULL,
                    idRegistradoraCobro INT NOT NULL,
                    TipoOperacion INT NOT NULL,
                    Procesado SMALLINT NOT NULL,
                    FechaHoraVenta DATETIME NOT NULL,
                    TipoVenta INT NOT NULL
                );",
                    "#TempVentas",
                    @"
                MERGE INTO Ventas AS target
                USING #TempVentas AS source
                ON target.FechaOperacion = source.FechaOperacion
                   AND target.ClaveSimi = source.ClaveSimi
                   AND target.Id_Venta = source.Id_Venta
                WHEN NOT MATCHED THEN
                    INSERT (FechaOperacion, ClaveSimi, Id_Venta, id_usuario_venta, Empleado,
                            idRegistradora, idRegistradoraVenta, idRegistradoraCobro, TipoOperacion,
                            Procesado, FechaHoraVenta, TipoVenta)
                    VALUES (source.FechaOperacion, source.ClaveSimi, source.Id_Venta, source.id_usuario_venta, source.Empleado,
                            source.idRegistradora, source.idRegistradoraVenta, source.idRegistradoraCobro, source.TipoOperacion,
                            source.Procesado, source.FechaHoraVenta, source.TipoVenta);");

               Console.WriteLine($"Ventas procesadas.");
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                // 2) VentasProductos
                var ventasProductos = dto.VentasProductos?.Where(v => v.Id_Venta != null).ToList();
               Console.WriteLine($"Procesando VentasProductos ({ventasProductos?.Count ?? 0})...");
                nombreProceso = "VentasProductos";
                await BulkMergeAsync(connection, ventasProductos, @"
                CREATE TABLE #TempVentasProductos (
                    FechaOperacion DATETIME NOT NULL,
                    ClaveSimi CHAR(10) NOT NULL,
                    Id_Venta INT NOT NULL,
                    Codigo CHAR(10) NOT NULL,
                    Id_ProductoSAT VARCHAR(20) NOT NULL,
                    TipoOperacion INT NOT NULL,
                    Producto VARCHAR(255) NOT NULL,
                    NoPonderado BIT NOT NULL,
                    Premio BIT NOT NULL,
                    Combo BIT NOT NULL,
                    Inventario BIT NOT NULL,
                    Cantidad DECIMAL(10,2) NOT NULL,
                    Precio DECIMAL(10,2) NOT NULL,
                    IVA DECIMAL(10,2) NOT NULL,
                    Descuento DECIMAL(10,2) NOT NULL,
                    DescuentoPorciento DECIMAL(10,2) NOT NULL,
                    IVA_Porciento DECIMAL(10,2) NOT NULL,
                    IVA_Importe DECIMAL(10,2) NOT NULL,
                    Presentacion VARCHAR(50) NULL,
                    Nivel1 VARCHAR(50) NOT NULL,
                    Nivel2 VARCHAR(50) NOT NULL,
                    Nivel3 VARCHAR(50) NOT NULL
                );",
                    "#TempVentasProductos",
                    @"
                MERGE INTO VentasProductos AS target
                USING #TempVentasProductos AS source
                ON target.FechaOperacion = source.FechaOperacion
                   AND target.ClaveSimi = source.ClaveSimi
                   AND target.Id_Venta = source.Id_Venta
                   AND target.Codigo = source.Codigo

                WHEN MATCHED THEN
                    UPDATE SET
                        target.Id_ProductoSAT     = source.Id_ProductoSAT,
                        target.TipoOperacion      = source.TipoOperacion,
                        target.Producto           = source.Producto,
                        target.NoPonderado        = source.NoPonderado,
                        target.Premio             = source.Premio,
                        target.Combo              = source.Combo,
                        target.Inventario         = source.Inventario,
                        target.Cantidad           = source.Cantidad,
                        target.Precio             = source.Precio,
                        target.IVA                = source.IVA,
                        target.Descuento          = source.Descuento,
                        target.DescuentoPorciento = source.DescuentoPorciento,
                        target.IVA_Porciento      = source.IVA_Porciento,
                        target.IVA_Importe        = source.IVA_Importe,
                        target.Presentacion       = source.Presentacion,
                        target.Nivel1             = source.Nivel1,
                        target.Nivel2             = source.Nivel2,
                        target.Nivel3             = source.Nivel3

                WHEN NOT MATCHED THEN
                    INSERT (
                        FechaOperacion, ClaveSimi, Id_Venta, Codigo,
                        Id_ProductoSAT, TipoOperacion, Producto,
                        NoPonderado, Premio, Combo, Inventario,
                        Cantidad, Precio, IVA, Descuento,
                        DescuentoPorciento, IVA_Porciento, IVA_Importe,
                        Presentacion, Nivel1, Nivel2, Nivel3
                    )
                    VALUES (
                        source.FechaOperacion, source.ClaveSimi, source.Id_Venta, source.Codigo,
                        source.Id_ProductoSAT, source.TipoOperacion, source.Producto,
                        source.NoPonderado, source.Premio, source.Combo, source.Inventario,
                        source.Cantidad, source.Precio, source.IVA, source.Descuento,
                        source.DescuentoPorciento, source.IVA_Porciento, source.IVA_Importe,
                        source.Presentacion, source.Nivel1, source.Nivel2, source.Nivel3
                    );
                ");

               Console.WriteLine($"VentasProductos procesadas.");
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                // 3) VentasImpuestos
                var ventasImpuestos = dto.VentasImpuestos?.Where(v => v.Id_Venta != null).ToList();
                nombreProceso = "VentasImpuestos";

               Console.WriteLine($"Procesando VentasImpuestos ({ventasImpuestos?.Count ?? 0})...");
                await BulkMergeAsync(connection, ventasImpuestos, @"
                CREATE TABLE #TempVentasImpuestos (
                    FechaOperacion DATETIME NOT NULL,
                    ClaveSimi CHAR(10) NOT NULL,
                    Id_Venta INT NOT NULL,
                    Impuesto VARCHAR(10) NOT NULL,
                    TipoFactor VARCHAR(10) NOT NULL,
                    TasaImpuesto NUMERIC(12,2) NOT NULL,
                    ClaveSATImpuesto VARCHAR(10) NOT NULL,
                    BaseImpuesto NUMERIC(12,2) NOT NULL,
                    ImporteImpuesto NUMERIC(12,2) NOT NULL,
                    TipoOperacion INT NOT NULL
                );",
                    "#TempVentasImpuestos",
                    @"
                MERGE INTO VentasImpuestos AS target
                USING #TempVentasImpuestos AS source
                ON target.FechaOperacion = source.FechaOperacion
                   AND target.ClaveSimi = source.ClaveSimi
                   AND target.Id_Venta = source.Id_Venta
                   AND target.Impuesto = source.Impuesto
                   AND target.TipoFactor = source.TipoFactor
                   AND target.TasaImpuesto = source.TasaImpuesto
                WHEN NOT MATCHED THEN
                    INSERT (FechaOperacion, ClaveSimi, Id_Venta, Impuesto, TipoFactor, TasaImpuesto, ClaveSATImpuesto, BaseImpuesto, ImporteImpuesto, TipoOperacion)
                    VALUES (source.FechaOperacion, source.ClaveSimi, source.Id_Venta, source.Impuesto, source.TipoFactor, source.TasaImpuesto, source.ClaveSATImpuesto, source.BaseImpuesto, source.ImporteImpuesto, source.TipoOperacion);");

               Console.WriteLine($"VentasImpuestos procesadas.");
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                // 4) VentasImpuestosDetalle
                var ventasImpuestosDetalle = dto.VentasImpuestosDetalle?.Where(v => v.Id_Venta != null).ToList();
               Console.WriteLine($"Procesando VentasImpuestosDetalle ({ventasImpuestosDetalle?.Count ?? 0})...");
                nombreProceso = "VentasImpuestosDetalle";

                await BulkMergeAsync(connection, ventasImpuestosDetalle, @"
                CREATE TABLE #TempVentasImpuestosDetalle (
                    ClaveSimi CHAR(10) NOT NULL,
                    FechaOperacion DATETIME NOT NULL,
                    Id_Venta INT NOT NULL,
                    Id_Producto VARCHAR(10) NOT NULL,
                    Impuesto VARCHAR(10) NOT NULL,
                    ClaveImpuesto VARCHAR(10) NOT NULL,
                    TasaImpuesto NUMERIC(12,2) NOT NULL,
                    TipoFactor VARCHAR(10) NOT NULL,
                    Base NUMERIC(12,2) NOT NULL,
                    ImporteIVA NUMERIC(12,2) NOT NULL,
                    ImporteVenta NUMERIC(12,2) NOT NULL,
                    TipoOperacion INT NOT NULL
                );",
                    "#TempVentasImpuestosDetalle",
                    @"
                MERGE INTO VentasImpuestosDetalle AS target
                USING #TempVentasImpuestosDetalle AS source
                ON target.FechaOperacion = source.FechaOperacion
                   AND target.ClaveSimi = source.ClaveSimi
                   AND target.Id_Venta = source.Id_Venta
                   AND target.Id_Producto = source.Id_Producto
                   AND target.Impuesto = source.Impuesto
                WHEN NOT MATCHED THEN
                    INSERT (ClaveSimi, FechaOperacion, Id_Venta, Id_Producto, Impuesto, ClaveImpuesto, TasaImpuesto, TipoFactor, Base, ImporteIVA, ImporteVenta, TipoOperacion)
                    VALUES (source.ClaveSimi, source.FechaOperacion, source.Id_Venta, source.Id_Producto, source.Impuesto, source.ClaveImpuesto, source.TasaImpuesto, source.TipoFactor, source.Base, source.ImporteIVA, source.ImporteVenta, source.TipoOperacion);");

               Console.WriteLine($"VentasImpuestosDetalle procesadas.");
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                // 5) VentasDesgloceTotales
                var ventasDesgloceTotales = dto.VentasDesgloceTotales?.Where(v => v.Id_Venta != null).ToList();
               Console.WriteLine($"Procesando VentasDesgloseTotales ({ventasDesgloceTotales?.Count ?? 0})...");
                nombreProceso = "VentasDesgloseTotales";
                await BulkMergeAsync(connection, ventasDesgloceTotales, @"
                CREATE TABLE #VentasDesgloseTotales (
                    ClaveSimi CHAR(10) NOT NULL,
                    FechaOperacion DATETIME NOT NULL,
                    Id_Venta INT NOT NULL,
                    PrecioSinIVA NUMERIC(12,2) NOT NULL,
                    Importe NUMERIC(12,2) NOT NULL,
                    Descuento NUMERIC(12,2) NOT NULL,
                    Impuestos NUMERIC(12,2) NOT NULL,
                    Total NUMERIC(12,2) NOT NULL,
                    TipoOperacion INT NOT NULL
                );",
                    "#VentasDesgloseTotales",
                    @"
                MERGE INTO VentasDesgloseTotales AS target
                USING #VentasDesgloseTotales AS source
                ON target.FechaOperacion = source.FechaOperacion
                   AND target.ClaveSimi = source.ClaveSimi
                   AND target.Id_Venta = source.Id_Venta
                WHEN NOT MATCHED THEN
                    INSERT (ClaveSimi, FechaOperacion, Id_Venta, PrecioSinIVA, Importe, Descuento, Impuestos, Total, TipoOperacion)
                    VALUES (source.ClaveSimi, source.FechaOperacion, source.Id_Venta, source.PrecioSinIVA, source.Importe, source.Descuento, source.Impuestos, source.Total, source.TipoOperacion);");

               Console.WriteLine($"VentasDesgloseTotales procesadas.");
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                // 6) VentasImportesProductos
                var ventasImportesProductos = dto.VentasImportesProductos?.Where(v => v.Id_Venta != null).ToList();
               Console.WriteLine($"Procesando VentasImportesProductos ({ventasImportesProductos?.Count ?? 0})...");
                nombreProceso = "VentasImportesProductos";

                await BulkMergeAsync(connection, ventasImportesProductos, @"
                CREATE TABLE #TempVentasImportesProductos (
                    FechaOperacion DATETIME NOT NULL,
                    ClaveSimi CHAR(10) NOT NULL,
                    Id_Venta INT NOT NULL,
                    Id_Producto VARCHAR(10) NOT NULL,
                    Precio NUMERIC(12,2) NOT NULL,
                    PrecioUnitarioNeto NUMERIC(18,4) NOT NULL,
                    Cantidad INT NOT NULL,
                    SubtotalNeto NUMERIC(18,4) NOT NULL,
                    SubtotalConImpuestos NUMERIC(18,4) NOT NULL,
                    DescuentoNeto NUMERIC(18,4) NOT NULL,
                    DescuentoConImpuestos NUMERIC(18,4) NOT NULL,
                    ImporteNeto NUMERIC(12,2) NOT NULL,
                    ImporteConImpuestos NUMERIC(12,2) NOT NULL,
                    ImpuestoCalculado NUMERIC(18,4) NOT NULL,
                    Total NUMERIC(12,2) NOT NULL,
                    TipoOperacion INT NOT NULL
                );",
                    "#TempVentasImportesProductos",
                    @"
                MERGE INTO VentasImportesProductos AS target
                USING #TempVentasImportesProductos AS source
                ON target.FechaOperacion = source.FechaOperacion
                   AND target.ClaveSimi = source.ClaveSimi
                   AND target.Id_Venta = source.Id_Venta
                   AND target.Id_Producto = source.Id_Producto
                WHEN NOT MATCHED THEN
                    INSERT (FechaOperacion, ClaveSimi, Id_Venta, Id_Producto, Precio, PrecioUnitarioNeto, Cantidad,
                            SubtotalNeto, SubtotalConImpuestos, DescuentoNeto, DescuentoConImpuestos, ImporteNeto, ImporteConImpuestos,
                            ImpuestoCalculado, Total, TipoOperacion)
                    VALUES (source.FechaOperacion, source.ClaveSimi, source.Id_Venta, source.Id_Producto, source.Precio, source.PrecioUnitarioNeto, source.Cantidad,
                            source.SubtotalNeto, source.SubtotalConImpuestos, source.DescuentoNeto, source.DescuentoConImpuestos, source.ImporteNeto, source.ImporteConImpuestos,
                            source.ImpuestoCalculado, source.Total, source.TipoOperacion);");

               Console.WriteLine($"VentasImportesProductos procesadas.");
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                // 7) VentasVendedorCuotas
                Console.WriteLine($"Procesando VentasVendedorCuotas ({dto.VentasVendedorCuotas?.Count ?? 0})...");
                nombreProceso = "VentasVendedorCuotas";

                var ventasVendedorCuotasConSucursal = dto.VentasVendedorCuotas
                    .Select(v => new VentasVendedorCuotasDto
                    {
                        ClaveSimi = sucursal,
                        Fecha = v.Fecha,
                        IdVendedor = v.IdVendedor,
                        Nombre = v.Nombre,
                        ImporteVenta = v.ImporteVenta,
                        Transaccionesventa = v.Transaccionesventa,
                        PorcVenta = v.PorcVenta,
                        ImporteNaturistas = v.ImporteNaturistas,
                        PorcNaturistas = v.PorcNaturistas,
                        ImporteNocturno = v.ImporteNocturno,
                        MontoDescuento = v.MontoDescuento,
                        Menudeos = v.Menudeos,
                        MontoIva = v.MontoIva
                    }).ToList();

                await BulkMergeAsync(connection, ventasVendedorCuotasConSucursal, @"
                                    CREATE TABLE #TempVentasVendedorCuotas (
                                        ClaveSimi VARCHAR(6) NOT NULL,
                                        Fecha DATETIME NOT NULL,
                                        IdVendedor VARCHAR(10) NOT NULL,
                                        Nombre VARCHAR(200) NOT NULL,
                                        ImporteVenta DECIMAL(12,2) NOT NULL,
                                        Transaccionesventa INT NOT NULL,
                                        PorcVenta DECIMAL(12,2) NOT NULL,
                                        ImporteNaturistas DECIMAL(12,2) NOT NULL,
                                        PorcNaturistas DECIMAL(12,2) NOT NULL,
                                        ImporteNocturno DECIMAL(12,2) NOT NULL,
                                        MontoDescuento DECIMAL(12,2) NOT NULL,
                                        Menudeos DECIMAL(12,2) NOT NULL,
                                        MontoIva DECIMAL(12,2) NOT NULL
                                    );",
                                                    "#TempVentasVendedorCuotas",
                                                    @"
                                    MERGE INTO VentasVendedorCuotas AS target
                                    USING #TempVentasVendedorCuotas AS source
                                    ON target.ClaveSimi = source.ClaveSimi
                                       AND CONVERT(VARCHAR,target.Fecha,112) = CONVERT(VARCHAR,source.Fecha,112)
                                       AND target.IdVendedor = source.IdVendedor

                                    WHEN MATCHED
                                    THEN UPDATE SET 
                                            target.Nombre = source.Nombre,
                                            target.ImporteVenta = source.ImporteVenta,
                                            target.Transaccionesventa = source.Transaccionesventa,
                                            target.PorcVenta = source.PorcVenta,
                                            target.ImporteNaturistas = source.ImporteNaturistas,
                                            target.PorcNaturistas = source.PorcNaturistas,
                                            target.ImporteNocturno = source.ImporteNocturno,
                                            target.MontoDescuento = source.MontoDescuento,
                                            target.Menudeos = source.Menudeos,
                                            target.MontoIva = source.MontoIva

                                    WHEN NOT MATCHED THEN
                                        INSERT (
                                            ClaveSimi, Fecha, IdVendedor, Nombre,
                                            ImporteVenta, Transaccionesventa, PorcVenta,
                                            ImporteNaturistas, PorcNaturistas, ImporteNocturno,
                                            MontoDescuento, Menudeos, MontoIva
                                        )
                                        VALUES (
                                            source.ClaveSimi, source.Fecha, source.IdVendedor, source.Nombre,
                                            source.ImporteVenta, source.Transaccionesventa, source.PorcVenta,
                                            source.ImporteNaturistas, source.PorcNaturistas, source.ImporteNocturno,
                                            source.MontoDescuento, source.Menudeos, source.MontoIva
                                        );
                                    ");

                Console.WriteLine($"VentasVendedorCuotas procesadas.");
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                connection.Dispose();


                using (var conn = new MySqlConnection(conextionStringFacturaRealOrquestador))
                {
                    conn.Open();
                    string query = @"UPDATE soltec2_Historicos SET Estatus = 'PROCESADO', Activo = 0, FechaProcesado=SYSDATE() WHERE ClaveSimi = @ClaveSimi AND IdHistorico=@IdHistorico";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClaveSimi", sucursal);
                        cmd.Parameters.AddWithValue("@IdHistorico", id);
                        cmd.ExecuteNonQuery();
                    }
                    

                    conn.Close();
                }

               Console.WriteLine($"Sincronización completada correctamente.");
                return new Response<TransmisionHistorico> { Exito = true, Mensaje = "Carga de históricos exitoso." };
            }
            catch (Exception ex)
            {
                using (var conn = new MySqlConnection(conextionStringFacturaRealOrquestador))
                {
                    conn.Open();
                    string query = @"UPDATE soltec2_Historicos SET Estatus = 'RECIBIDO' WHERE ClaveSimi = @ClaveSimi AND IdHistorico=@IdHistorico";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClaveSimi", sucursal);
                        cmd.Parameters.AddWithValue("@IdHistorico", id);
                        cmd.ExecuteNonQuery();
                    }


                    conn.Close();
                }
                return new Response<TransmisionHistorico> { Exito = false, Mensaje = ex.Message };
            }
        }


        private async Task BulkMergeAsync<T>(
                                               SqlConnection connection,
                                               //SqlTransaction transaction,
                                               IEnumerable<T> data,
                                               string tempTableSql,
                                               string tempTableName,
                                               string mergeSql,
       int batchSizeOverride = -1)
        {
            if (data == null) return;
            var list = data as IList<T> ?? data.ToList();
            if (!list.Any()) return;

            int effectiveBatch = batchSizeOverride > 0 ? batchSizeOverride : batchSize;

            // Crear tabla temporal en la sesión (dentro de la transacción)
            using (var createCmd = new SqlCommand(tempTableSql, connection))
            {
                createCmd.CommandTimeout = 120;
                await createCmd.ExecuteNonQueryAsync();
            }

            // Convertir a DataTable (prop names => column names)
            var table = ToDataTable(list);

            // Cargar en bloques
            int totalRows = table.Rows.Count;
            int processed = 0;
            for (int i = 0; i < totalRows; i += effectiveBatch)
            {
                var rows = table.AsEnumerable().Skip(i).Take(effectiveBatch).CopyToDataTable();
                using (var bulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, null))
                {
                    bulk.DestinationTableName = tempTableName;
                    bulk.BulkCopyTimeout = 0; // sin timeout (ajusta si lo deseas)
                    bulk.EnableStreaming = true;
                    await bulk.WriteToServerAsync(rows);
                }
                processed += rows.Rows.Count;
                //Logger.Important($"    Cargadas {processed}/{totalRows} filas en {tempTableName}");
            }

            // Ejecutar MERGE
            using (var mergeCmd = new SqlCommand(mergeSql, connection))
            {
                mergeCmd.CommandTimeout = 600;
                int affected = await mergeCmd.ExecuteNonQueryAsync();
                //Logger.Important($"    MERGE completado en {tempTableName} - filas afectadas (ExecuteNonQuery): {affected}");
            }

            // Borrar temp table (opcional, pero limpio)
            using (var dropCmd = new SqlCommand($"DROP TABLE IF EXISTS {tempTableName};", connection))
            {
                await dropCmd.ExecuteNonQueryAsync();
            }
        }


        private static DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            var dt = new DataTable();
            var props = typeof(T).GetProperties();

            // Column names must match exactly the properties used in the temp table
            foreach (var p in props)
            {
                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                dt.Columns.Add(p.Name, type);
            }

            foreach (var item in items)
            {
                var values = props.Select(p => p.GetValue(item) ?? DBNull.Value).ToArray();
                dt.Rows.Add(values);
            }

            return dt;
        }
    }
}
