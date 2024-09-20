using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Models
{
    public class HomeViewModel
    {
        public List<Local> Stores { get; set; }
        public string StoreIdSelected { get; set; }
        public string StoreIdInputValue { get; set; }

        public HomeViewModel()
        {
            Stores = new List<Local>();
        }
        public HomeViewModel(List<Local> stores)
        {
            Stores = stores;
        }
    }
}