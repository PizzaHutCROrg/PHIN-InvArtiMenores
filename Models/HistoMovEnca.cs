using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Models
{
    public class HistoMovEnca
    {
        public string NumDoc { get; set; }
        public string Proveedor { get; set; }
        public string Motivo { get; set; }
        public string TipoMov { get; set; }
        public string Username { get; set; } //= string.Empty;
        public string CodRest { get; set; }
        public List<HistoMov> Detalles { get; set; }
       // public HistoMov Detalles { get; set; }
       public string Responsable { get; set; } = string.Empty;

    }
}