using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Models.Report
{
    public class KardexViewModel
    {
        public string CodRest { get; set; }
        public string CodMateria { get; set; }
        public string DesMateria { get; set; }
        // public string TipoMov { get; set; }
        public decimal Cantidad { get; set; }
        public string Comentario { get; set; }
        public string NumDoc { get; set; }
        public string Proveedor { get; set; }
        public string Responsable { get; set; }
        public string Motivo { get; set; }
        //  [DisplayFormat(DataFormatString = "{0:n4}")]
        public decimal Costo { get; set; }
        //  [DisplayFormat(DataFormatString = "{0:n4}")]
        public decimal Monto { get; set; } = 0;
        public decimal Acumulado { get; set; } = 0;
        public DateTime FechaReg { get; set; }
    }
}