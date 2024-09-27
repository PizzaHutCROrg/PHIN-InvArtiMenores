using InventarioArtMenores.Models;
using InventarioArtMenores.Models.Report;
using InventarioArtMenores.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventarioArtMenores.Controllers
{
    public class ReportController : Controller
    {
        RepoArticulo repoArti = new RepoArticulo();
        RepoControl repoControl = new RepoControl();
        RepoInventario repoInv = new RepoInventario();
        RepoHistorico repoHist = new RepoHistorico();
        public static string tipoInv = "Menores";
 
        public ActionResult Index(string txtNumTF, DateTime? fechaInicio, DateTime? fechaFin)
        {
            //Session["UserInvAM"] = "test";//quitar
            //  Session["codResArti"] = "40";//quitar
            //  Session["desResArti"] = "Rohrmoser";//quitar

            if (Session["UserInvAM"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                if (Session["codResArti"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }

            }

            //var reportes = repoInv.GetAllNumTF(Session["codResArti"].ToString(), Convert.ToInt32(txtNumTF), tipoInv);

            ////if (!string.IsNullOrEmpty(filtroEstado) && filtroEstado != "All")
            ////{
            ////    reportes = reportes.Where(r => r.NumTF == 1).ToList();
            ////}
            //if (fechaInicio.HasValue)
            //{
            //    reportes = reportes.Where(r => r.FechaAudi >= fechaInicio.Value).ToList();
            //}

            //if (fechaFin.HasValue)
            //{
            //    reportes = reportes.Where(r => r.FechaAudi <= fechaFin.Value).ToList();
            //}

            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            ViewBag.FiltrotxtNumTF = txtNumTF;
            ViewBag.FechaMaxima = DateTime.Now.ToString("yyyy-MM-dd"); // Obtener la fecha actual

            //return View(reportes);
            return View();
        }
        //public List<Inventario> GetReportes()
        //{
        //    return new List<Inventario> { };
        //}


        public List<Inventario> ObtenerListaDeArticulos(string codRest, int txtNum)
        {
            // lista de artículos de ese inventario q corresponde a esa toma física
            List<Inventario> articulos = new List<Inventario>();
            articulos = repoInv.GetAllNumTF(codRest, txtNum, tipoInv);
            return articulos;
        }


        public JsonResult ObtenerArticulos(string txtNumTF, DateTime? fechaInicio, DateTime? fechaFin, string sortField, string sortOrder)
        {
            string codRestS = Session["codResArti"].ToString();
            var articulos = new List<Inventario>();

            if (!string.IsNullOrEmpty(txtNumTF))
            {
                articulos = ObtenerListaDeArticulos(codRestS, Convert.ToInt32(txtNumTF)); // Método que obtiene la lista original de productos
                                                                                          // articulos = articulos.Where(p => p.CodMateria.ToString().Contains(CodMateria.Trim().ToUpper())).ToList();
            }
            //if (!string.IsNullOrEmpty(NomMateria))
            //{
            //    articulos = articulos.Where(p => p.NomMateria.ToString().Contains(NomMateria.Trim().ToUpper())).ToList();
            //}
            /* if (Factor.HasValue)
             {
                 articulos = articulos.Where(p => p.Factor == Factor.Value).ToList();
             }
             if (Teorico.HasValue)
             {
                 articulos = articulos.Where(p => p.Teorico == Teorico.Value).ToList();
             }
             if (Fisico.HasValue)
             {
                 articulos = articulos.Where(p => p.Fisico == Fisico.Value).ToList();
             }*/
            //if (Diferencia.HasValue)
            //{
            //    articulos = articulos.Where(p => p.Diferencia == Diferencia.Value).ToList();
            //}
            // Retornar los productos y el mensaje adicional como parte del JSON
            //return Json(new { articulos = articulos, mensaje = mensaje }, JsonRequestBehavior.AllowGet);

            // Aplicar el ordenamiento basado en sortField y sortOrder //prueba con el select
            //switch (sortField)
            //{
            //    case "ObjArticulo.CodMateria":
            //        articulos = sortOrder == "asc" ?
            //            articulos.OrderBy(i => i.ObjArticulo.CodMateria).ToList() :
            //            articulos.OrderByDescending(i => i.ObjArticulo.CodMateria).ToList();
            //        break;
            //    case "ObjArticulo.NomMateria":
            //        articulos = sortOrder == "asc" ?
            //            articulos.OrderBy(i => i.ObjArticulo.NomMateria).ToList() :
            //            articulos.OrderByDescending(i => i.ObjArticulo.NomMateria).ToList();
            //        break;
            //        // Añade más casos para otras columnas según sea necesario
            //}

            return Json(articulos, JsonRequestBehavior.AllowGet);
        }


        //***************************************************************************************************************
        //************************************** Kardex *****************************************************************
        //***************************************************************************************************************

        public ActionResult Kardex()
        {

            //Session["UserInvAM"] = "test";//quitar
            //Session["UserNomInvAM"] = "Yess-test";//quitar
            //Session["codResArti"] = "40"; //"36";//quitar
            //Session["desResArti"] = "Rohrmoser";//"Cartago"; ///quitar

            if (Session["UserInvAM"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["codResArti"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.OptionsList = repoArti.GetListAllArticulo(Session["codResArti"].ToString());//obtener la lista de string con los articulos de la tienda
            }
            return View();
        }

        public List<KardexViewModel> KarGetLista(string codRest, string txtNum)
        {
            List<KardexViewModel> articulos = new List<KardexViewModel>();
            articulos = repoHist.KarGetHistArticulo(codRest, txtNum);
            return articulos;
        }

        [HttpPost]
        public JsonResult KarGetArticulo(string codigo)
        {
            string codRestS = Session["codResArti"].ToString();
            var articulos = new List<KardexViewModel>();
            string[] arti = codigo.Split('-');//codigo-desc

            if (!string.IsNullOrEmpty(arti[0]))
            {
                articulos = KarGetLista(codRestS, arti[0]); // Método que obtiene la lista original de productos                                                                                      
            }
            
            //return Json(new { ltsArticulos = articulos, cod = arti[0], desc = arti[1] }, JsonRequestBehavior.AllowGet);//no funka
            return Json(articulos);
        }

        //***************************************************************************************************************
        //************************************** Saldos *****************************************************************
        //***************************************************************************************************************

        public ActionResult Saldos()
        {

            //Session["UserInvAM"] = "test";//quitar
            //Session["UserNomInvAM"] = "Yess-test";//quitar
            //Session["codResArti"] = "40"; //"36";//quitar
            //Session["desResArti"] = "Rohrmoser";//"Cartago"; ///quitar

            if (Session["UserInvAM"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["codResArti"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.OptionsList = repoArti.GetListAllArticulo(Session["codResArti"].ToString());//obtener la lista de string con los articulos de la tienda
            }
            return View();
        }

        public List<SaldosViewModel> SalGetLista(string codRest)
        {
            List<SaldosViewModel> articulos = new List<SaldosViewModel>();
            articulos = repoHist.SalGetHistArticulo(codRest);
            return articulos;
        }

        [HttpPost]
        public JsonResult SalGetArticulo(string CodMateria, string NomMateria)
       {
            string codRestS = Session["codResArti"].ToString();
            var articulos = new List<SaldosViewModel>();
            articulos = SalGetLista(codRestS); // Método que obtiene la lista original de productos                   
            //return Json(new { ltsArticulos = articulos, cod = arti[0], desc = arti[1] }, JsonRequestBehavior.AllowGet);//no funka

            if (!string.IsNullOrEmpty(CodMateria))
            {
                articulos = articulos.Where(p => p.CodMateria.ToString().Contains(CodMateria.Trim().ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(NomMateria))
            {
                articulos = articulos.Where(p => p.NomMateria.ToString().Contains(NomMateria.Trim().ToUpper())).ToList();
            }

            if (articulos.Count == 0)
            {
                TempData["Message"] = "No hay registros";
            }
            return Json(articulos);
        }

    }
}