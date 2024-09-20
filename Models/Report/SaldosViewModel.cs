using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Models.Report
{
    public class SaldosViewModel
    {
        public string CodRest { get; set; }
        public string CodMateria { get; set; }
        public string NomMateria { get; set; }
        public bool Estado { get; set; }
        public decimal Minimo { get; set; }
        public decimal Maximo { get; set; }
        public string Tipo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }
        public decimal Monto { get; set; }
        public DateTime UC_Fecha { get; set; }
        public string TipoNivelInv { get; set; }
       // public decimal TipoNivelInv { get; set; }
        public decimal UC_Cantidad { get; set; }
        public decimal UC_Costo { get; set; }
        public decimal UC_Monto { get; set; }
        public SaldosViewModel() { }
    }
}