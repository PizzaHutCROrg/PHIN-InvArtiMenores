using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Models
{
    public class Articulo
    {
        public string CodMateria { set; get; }
        public string NomMateria { set; get; }
        public string CodRest { set; get; }
        public string NomRest { set; get; }
        public bool Estado { set; get; }
        public string Tipo { set; get; }
        public decimal Factor { set; get; }
        public string Promedio { set; get; }
        public decimal Minimo { set; get; }
        public decimal Maximo { set; get; }
        public bool Conteo { set; get; }
        public string CodLinea { set; get; }


        public Articulo() { }   
    }
}