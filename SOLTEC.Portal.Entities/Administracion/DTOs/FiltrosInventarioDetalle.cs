using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLTEC.Portal.Entities.Administracion.DTOs
{
    public class FiltrosInventarioDetalle
    {
        public DateTime Desde { get; set; }
        public List<string>? ClavesSIMI { get; set; }

    }
}
