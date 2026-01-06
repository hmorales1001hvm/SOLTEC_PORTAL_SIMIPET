using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLTEC.Portal.Entities
{
    public class Response<T>
    {
        public bool Exito { get; set; }          
        public string Mensaje { get; set; }      
        public T? Datos { get; set; }
        public object? DatosLista { get; set; }
        public string? CodigoError { get; set; } 
    }
}
