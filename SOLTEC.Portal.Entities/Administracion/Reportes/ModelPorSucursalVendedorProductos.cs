using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLTEC.Portal.Entities.Administracion.Reportes
{
    public class ModelPorSucursalVendedorProductos
    {
        public string? Sucursal { get; set; }
        public string? ClaveSimi { get; set; }
        public string? Vendedor { get; set; }
        public string? Producto { get; set; }
        public int Piezas { get; set; }
        public decimal Descuento { get; set; }
        public decimal IVA { get; set; }
        public decimal VentaNeta { get; set; }
    }
}
