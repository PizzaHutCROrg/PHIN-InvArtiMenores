using Antlr.Runtime.Misc;
using InventarioArtMenores.Models;
using InventarioArtMenores.Repos;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventarioArtMenores.Controllers
{
    public class HomeController : Controller
    {
        RepoArticulo repoArti = new RepoArticulo();
        RepoControl repoControl = new RepoControl();
        RepoInventario repoInv = new RepoInventario();
        RepoHistorico repoHist = new RepoHistorico();
        RepoLocal repoLocal = new RepoLocal();
        RepoUsuario repoUsuario = new RepoUsuario();
        public static string tipoInv = "Menores";
       
        public ActionResult Index()
        {
             Session["codResArti"] = null;//código de la tienda seleccionada           
             Session["desResArti"] = null;

            //Session["UserInvAM"] = "test";//quitar
            if (Session["UserInvAM"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                //obtenemos la lista de los locales asociados
               List<string> ltsCodRest = repoUsuario.GetUserLocal(Session["UserInvAM"].ToString());

                ViewBag.Tiendas = new SelectList(repoLocal.GetAllLocalesCBX(ltsCodRest), "filtrocbx", "filtrocbx");
                return View();
            }
        }

        [HttpPost]
        public ActionResult Index(string cmbTiendas2) //HomeViewModel model //txtTienda
        {
            if(cmbTiendas2 != null || !string.IsNullOrEmpty(cmbTiendas2)){
                string[] words = cmbTiendas2.Split('-');
                Session["codResArti"] = words[0];//código de la tienda seleccionada           
                Session["desResArti"] = words[1];//código de la tienda seleccionada      
                return RedirectToAction("Inventario");
            }
            TempData["Message"] = "Debe seleccionar un restaurante";
            return View();
           
        }



        public ActionResult Inventario()
        {

            //Session["UserInvAM"] = "test";//quitar
            //Session["UserNomInvAM"] = "Yess-test";//quitar
            //Session["codResArti"] = "36";//"40"; //quitar
            //Session["desResArti"] = "Cartago"; //"Rohrmoser";///quitar


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
                string codRest = Session["codResArti"].ToString();    
                int num = 0;
                Control control = repoControl.GetControlAbierto(codRest, tipoInv);//verificamos si existe un inventario abierto
                if (control.NumTF == 0)
                {
                    num = repoInv.GetInvNumTF(codRest, tipoInv);
                   // TempData["Abierto"] = "No hay Inventario Abierto";
                }
                else
                {
                    num = control.NumTF;
                    ViewBag.txtResp = control.Responsable;
                    TempData["txtRespon"] = control.Responsable;
                    TempData["Abierto"] = "Inventario Abierto";
                }
                    
                Session["NTF"] = num;
                return View();
            }
        }

       
        public List<ArticuloViewModel> ObtenerListaDeArticulos(string codRest, int txtNum)
        {
            // Aquí se obtiene la lista de artículos
            List<ArticuloViewModel> articulos = new List<ArticuloViewModel>();
            articulos = repoArti.GetListAllView(codRest,tipoInv, txtNum);
            return articulos;
        }


        public JsonResult ObtenerArticulos(string CodMateria, string NomMateria, decimal? Factor, decimal? Teorico, decimal? Fisico, decimal? Diferencia, string Estado /*,string CodRest, string txtNum, string txCodR*/)
        {
            string codRestS = Session["codResArti"].ToString();
            Control control = repoControl.GetControlAbierto(codRestS, tipoInv);//verificamos si existe un inventario abierto para obtener el NumTF
            // Dato adicional que quieres enviar a la vista
            
            var articulos = ObtenerListaDeArticulos(codRestS, control.NumTF); // Método que obtiene la lista original de productos

            if (!string.IsNullOrEmpty(Estado))//filtrado externo
            {
                switch (Estado)
                {
                    case "1"://Activos
                        articulos = articulos.Where(p => p.Estado == true && p.Conteo == true).ToList();
                        break;
                    case "2"://Sin Conteo (que no se deben de contar en el inventario)
                        articulos = articulos.Where(p => p.Conteo == false).ToList();
                        break;
                    case "3":// Con Conteo (se deben de contar en el inventario)
                        articulos = articulos.Where(p => p.Conteo == true).ToList();
                        break;
                    //case "4"://Sin Diferencia (no existe diferencia entre teórico y físico)//se puede hacer en el filtro del componente
                    //    productos = productos.Where(p => p.Diferencia == 0).ToList();
                    //    break;
                    case "4"://Con Diferencia (existe diferencia entre teórico y físico)
                        articulos = articulos.Where(p => p.Diferencia != 0).ToList();
                        break;
                    case "5"://Falta Conteo (los artículos que se debe de hacer conteo y aún no se ha realizado)
                        articulos = articulos.Where(p => p.Conteo == true && p.Fisico == 0).ToList();
                        break;
                    case "6"://contados (los que se han agregado en el inventario se modificó el físico)
                        articulos = articulos.Where(p => p.Fisico > 0).ToList();
                        break;
                    default:
                        break;

                }
            }

            if (!string.IsNullOrEmpty(CodMateria))
            {
                articulos = articulos.Where(p => p.CodMateria.ToString().Contains(CodMateria.Trim().ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(NomMateria))
            {
                articulos = articulos.Where(p => p.NomMateria.ToString().Contains(NomMateria.Trim().ToUpper())).ToList();
            }
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
            if (Diferencia.HasValue)
            {
                articulos = articulos.Where(p => p.Diferencia == Diferencia.Value).ToList();
            }
            // Retornar los productos y el mensaje adicional como parte del JSON
            //return Json(new { articulos = articulos, mensaje = mensaje }, JsonRequestBehavior.AllowGet);

           return Json(articulos, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ActualizarArticulos(ArticuloViewModel arti, string txtQuien)
        {
           // TempData["Abierto"] = "Inventario Abierto";
            int num = 0;
            bool banderaControl= false;
            List<decimal> montoCalculado = new List<decimal>();
            Control control = repoControl.GetControlAbierto(arti.CodRest, tipoInv);//verificamos si existe un inventario abierto
            if(control.NumTF == 0)
            {
                //no existe, se debe de insertar
                //obtener el siguiente NumTF
                num = repoInv.GetInvNumTF(arti.CodRest, tipoInv);
               
                if(Convert.ToInt32(Session["NTF"]) != num)
                {
                    Session["NTF"] = num;//sesion NTF
                }
                //insertamos en control
                control.NumTF = num;
                control.CodRest = arti.CodRest;
                control.Gerente = Session["UserNomInvAM"].ToString();
                control.Responsable = txtQuien;
                control.Turno = "Web"; 
                control.Tipo = tipoInv;
                control.UserName = Session["UserInvAM"].ToString();               
                if (repoControl.NewControl(control) > 0)//insertó en la tabla de control
                {
                    banderaControl = true;
                }
            }
            else
            {
                //existe
                banderaControl = true;
                num = control.NumTF;
            }
            
            if(banderaControl)//se insertó en la tabla de control
            {
                //buscar si la linea existe en el inventario
                int existe = repoInv.ExisteArtiEnInventario(arti.CodRest, num, arti.CodMateria, tipoInv);//obtenemos si existe el número de línea

                 //calculamos los montos
                //diferencia
                decimal diferencia = arti.Fisico-arti.Teorico;       
                //vamos a obtener costo y monto
                montoCalculado = repoHist.GetMontoCalculado(arti.CodMateria, Session["codResArti"].ToString(), diferencia);
                decimal montoCal = montoCalculado[1];
                decimal costoCal = montoCalculado[0];
                arti.Diferencia = diferencia;
                Inventario inv = new Inventario();
                inv.NumTF = num;
                inv.ObjArticulo = arti;
                //inv.ObjArticulo.CodRest = arti.CodRest;
                inv.FechaAudi = DateTime.Now;
                inv.Gerente = Session["UserNomInvAM"].ToString();
                inv.Responsable = txtQuien;
                inv.NumLinea = existe; 
                inv.CostoUni = costoCal;
                inv.MontoVenta = montoCal;
                inv.Tipo = tipoInv;
                inv.Turno = "";
                inv.UserName =Session["UserInvAM"].ToString();
                                        
                if (existe == 0)
                {
                    //no existe la línea agregada en el inventario, se debe de insertar
                    //obtener el número de línea
                    inv.NumLinea = repoInv.GetInvNumLinea(arti.CodRest, num, tipoInv,arti.CodMateria);

                    if (repoInv.AddInvArticulo(inv) == 0)//insertamos la línea
                    {
                        TempData["Message"] = "Ocurrió un error, en el proceso de agregar la línea al inventario";
                        return Json(new { success = false });
                    }
                }
                else
                {
                    //aqui va el update xq la linea ya existe en el inventario                   
                    if (repoInv.UpdateInvArticulo(inv) == 0)
                    {
                        TempData["Message"] = "Ocurrió un error, en el proceso de agregar la línea al inventario";
                        return Json(new { success = false });
                    }

                }
            }
            else
            {
                TempData["Message"] = "Ocurrió un error, en el proceso de agregar la línea";
                return Json(new { success = false });
            }
                   
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult CerrarInventario(int tipo, string txtQuien)//List<ArticuloViewModel> articulos
        {
            string codRestS = Session["codResArti"].ToString();
            string UserName = Session["UserInvAM"].ToString();
            if (tipo == 0)
            {
                TempData["Message"] = "Ocurrió un error, no se cerró el inventario";
                return Json(new { success = false });
            }
            if (string.IsNullOrEmpty(txtQuien))
            {
              txtQuien =  TempData["txtRespon"] as string;
            }
            Control control = repoControl.GetControlAbierto(codRestS, tipoInv);//verificamos si existe un inventario abierto
            if (control.NumTF > 0)
            {
                //se debe de cerrar el inventario en control
                if (repoControl.CerrarControlInventario(control.NumTF, codRestS, tipoInv, true, tipo, txtQuien) == 0)
                {
                    TempData["Message"] = "Ocurrió un error, no se cerró el control del inventario";
                    return Json(new { success = false });
                }
                //si se actualizó el estado en control, continúa con las lineas en histórico
                //se debe de insertar todas las q se ingresaron el usuario y las q no a la tabla de inventario
                List<ArticuloViewModel> todosArticulos = repoArti.GetListAllView(codRestS, tipoInv, control.NumTF);
                List<Inventario> lstArticulos = new List<Inventario>();
                List<decimal> montoCalculado = new List<decimal>();
                int contador = repoInv.GetInvNumLinea(codRestS, control.NumTF, tipoInv, "XX");
                foreach (var item in todosArticulos)
                {
                    //verifico si existe ya agregada
                    if (repoInv.ExisteArtiEnInventario(codRestS, control.NumTF,item.CodMateria, tipoInv) == 0)
                    {
                        contador++;
                        //vamos a obtener costo y monto
                        montoCalculado = repoHist.GetMontoCalculado(item.CodMateria, Session["codResArti"].ToString(), item.Diferencia);
                        decimal montoCal = montoCalculado[1];
                        decimal costoCal = montoCalculado[0];
                        //no existe la línea agregada en el inventario, se debe de insertar
                        //obtener el número de línea              
                        Inventario inv = new Inventario();
                        inv.NumTF = control.NumTF;
                        inv.ObjArticulo = item;
                        inv.FechaAudi = DateTime.Now;
                        inv.Gerente = Session["UserNomInvAM"].ToString();
                        inv.UserName = UserName;//Session["UserInvAM"].ToString();
                        inv.Responsable = txtQuien;
                        inv.NumLinea =contador;
                        inv.CostoUni = costoCal;
                        inv.MontoVenta = montoCal;
                        inv.Tipo = tipoInv;
                        inv.Turno = "Web";//falta                       
                        lstArticulos.Add(inv);//insertar la lista
                    }
                }
                //update del campo de Responsable del conteo, de las líneas ya agregadas  x el usuario 
                //txtQuien
                repoInv.UpdateInvResp(txtQuien, control.NumTF, codRestS);
                //para insertar la lista
                if (repoInv.AddInvLineaArticulo(lstArticulos) == 0)// beginTrans
                {
                    TempData["Message"] = "Ocurrió un error, cuando se agregaban al inventario";
                    return Json(new { success = false });
                }
                if (tipo == 5)//cerrar con ajuste
                {
                   //obtener todas las líneas agregadas por el usuario al inventario, que sea diferencias != 0 para ser insertadas en el histórico
                    List<ArticuloViewModel> articulos = repoInv.GetAllArtiEnInventario(codRestS,control.NumTF, tipoInv);              
                    if (repoHist.NewListHistorico(articulos, UserName) == 0)
                    {
                        repoControl.CerrarControlInventario(control.NumTF, codRestS, tipoInv, false, 7, txtQuien);
                        TempData["Message"] = "Ocurrió un error, en el proceso de agregar el cierre al histórico";
                        return Json(new { success = false });
                    }
                }               
            }
            else
            {
                TempData["Message"] = "No existe un inventario abierto, debe de abrir uno para realizar el cierre";
                return Json(new { success = false });
            }
            //actualizamos el estado del cierre en control
           // repoControl.CerrarControlInventario(control.NumTF, codRestS, tipoInv, false, tipo);
            TempData["Message"] = "Se realizó el cierre de forma exitosa";
           return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult CancelarInventario(int id)
        {
            // Lógica para eliminar el elemento con el ID proporcionado
            // ...
            if(id == 0)
            {
                TempData["Message"] = "Debe seleccionar un tipo de Cancelación";
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}
      

    }
}