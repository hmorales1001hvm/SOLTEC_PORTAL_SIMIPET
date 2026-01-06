using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLTEC.Portal.Entities.Administracion.Reportes
{
    public class ModelPorSucursalVendedor
    {
        public string? Sucursal { get; set; }
        public string? ClaveSimi { get; set; }
        public string? Vendedor { get; set; }
        public int TcksNetos { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal VentaNeta { get; set; }
        public decimal VentaBaseComision { get; set; }
        public decimal VentaConPremio { get; set; }
        public decimal PromedioXNota { get; set; }
    }
}
