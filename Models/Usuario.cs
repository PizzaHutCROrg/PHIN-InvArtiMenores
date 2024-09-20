using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Models
{
    public class Usuario
    {
        public string USER_NAME { get; set; }
        public string FULL_NAME { get; set; }
        //public string MAIL { get; set; }
        public int POINT_SALE { get; set; }
        //public int PROFILE_ID { get; set; }
        //public bool STATUS { get; set; }
        public List<Local> LtsLocales { get; set; }
       

    }
}