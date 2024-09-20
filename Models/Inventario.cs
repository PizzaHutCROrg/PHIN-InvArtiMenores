using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Models
{
    public class Inventario
    {
        public int NumTF { get; set; }      
        public DateTime FechaAudi {get; set;}
        public string Gerente {get; set;}
        public string Responsable { get; set; }
        public int NumLinea { get; set; }
        public decimal CostoUni { get; set; }
        public decimal MontoVenta { get; set; }
        public string Turno { get; set; }
        public string Tipo { get; set; }
        public string UserName { get; set; }
        public ArticuloViewModel ObjArticulo { get; set; }

    }
}