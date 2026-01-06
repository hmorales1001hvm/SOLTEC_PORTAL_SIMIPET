using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLTEC.Portal.Entities.Administracion.Reportes
{
    public class ModelInventarioValuacion
    {
        public string? Sucursal { get; set; }
        public string? Nombre { get; set; }
        public decimal Ventas { get; set; }
        public decimal Compras { get; set; }
        public decimal Existencias { get; set; }
    }
}
