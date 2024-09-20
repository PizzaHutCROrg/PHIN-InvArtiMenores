using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Models
{
    public class Control
    {
        public int NumTF { get; set; }
        public string CodRest { get; set; }
        public string Gerente { get; set; }
        public string Responsable { get; set; }
        public string Turno { get; set; }
        public string Tipo { get; set; }
        public bool Cerrado { get; set; }
        public DateTime FechaCerrado { get; set; }
        public string UserName { get; set; }
    }
}