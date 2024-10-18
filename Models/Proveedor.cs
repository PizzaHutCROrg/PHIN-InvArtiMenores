using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Models
{
    public class Proveedor
    {
        public string Descripcion { get; set; }
        public bool Activo { get; set; } = true;

    }
}