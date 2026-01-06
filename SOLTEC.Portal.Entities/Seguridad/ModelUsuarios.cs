using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLTEC.Portal.Entities.Seguridad
{
    public class ModelUsuarios
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string? Nombre { get; set; }
        public int IdUsuario { get; set; }
        public int IdEmpresa { get; set; }
        public List<ModelEmpresaSucursal>? EmpresaSucursal { get; set; }
    }

    public class ModelEmpresaSucursal
    {
        public string NombreEmpresa { get; set; }
        public string ClaveSimi { get; set; }
        public string? NombreSucursal { get; set; }
    }
}
