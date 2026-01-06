namespace SOLTEC.Portal.Entities.Administracion.Reportes
{
    public class ModelInventarioDetalle
    {
        public string? Sucursal { get; set; }
        public string? Nombre { get; set; }
        public string? Codigo { get; set; }
        public string? Producto { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal Existencia { get; set; }
    }
}
