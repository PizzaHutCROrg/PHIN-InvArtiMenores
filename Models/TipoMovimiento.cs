using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Models
{
    public class TipoMovimiento
    {
        public int TipoMov { get; set; }
        public string Descripcion { get; set; }
        public string filtrocbx { get; set; }
        public int idCategoria { get; set; } = 0;
        public bool Activo { get; set; }

    }
}