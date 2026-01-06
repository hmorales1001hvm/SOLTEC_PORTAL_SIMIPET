using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLTEC.Portal.Entities.Administracion
{
    public class ModelSucursales
    {
        public int IdSucursal { get; set; }
        public string? Sucursal { get; set; }
        public int IdEmpresa { get; set; }
        public string? Clave { get; set; }
        public bool Seleccionada { get; set; }
        public int IdUsuario { get; set; }
        public string? LlaveUnica { get; set; }
    }
}
