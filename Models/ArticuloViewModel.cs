using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Models
{
    public class ArticuloViewModel
    {
        public string CodMateria { set; get; } = string.Empty;
        public string NomMateria { set; get; } = string.Empty;
        public decimal Factor { set; get; } = decimal.Zero;
        public decimal Teorico { set; get; } = decimal.Zero;
        public decimal Fisico { set; get; } = decimal.Zero;
        public decimal Diferencia { set; get; } = decimal.Zero;
        public bool Estado { set; get; }
        public bool Conteo { set; get; }
        public string CodRest { set; get; } = string.Empty;
        public int NumTF { set; get; } = 0;
        //agregamos para calculos
       // MontoVenta, CostoUni
       public decimal MontoVenta { set; get; } = 0;
        public decimal CostoUni { set; get; } = 0;

        public ArticuloViewModel() { }
    }
}