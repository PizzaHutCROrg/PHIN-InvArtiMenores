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
        public static string tipoInv = "Menores";

        public ActionResult Index()
        {
            //UserInvAM
            Session["UserInvAM"] = "test";//quitar
            if (Session["UserInvAM"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                 string codRest = "40";//falta            
                /*Control control = repoControl.GetControlAbierto(codRest, tipoInv);//verificamos si existe un inventario abierto para obtener el NumTF
                ViewBag.Num = control.NumTF;
                ViewBag.CodR = codRest;
                List<ArticuloViewModel> articulos = new List<ArticuloViewModel>();
                articulos = repoArti.GetListAllView(codRest, tipoInv, control.NumTF);//se envía el tipo, codRest y el numTF si encontró, sino será 0             
                return View(articulos);*/
                Session["CodRe"] = codRest;
                int num = 0;
                Control control = repoControl.GetControlAbierto(codRest, tipoInv);//verificamos si existe un inventario abierto
                if (control.NumTF == 0)
                {
                    num = repoInv.GetInvNumTF(codRest, tipoInv);
                }
                else
                {
                    num = control.NumTF;
                }
                    
                Session["NTF"] = num;
               // ViewBag.NTF = num;//quitar
                return View();
            }
        }

       
        public List<ArticuloViewModel> ObtenerListaDeArticulos(string codRest, int txtNum)
        {
            // Aquí tienes una lista estática de productos para propósitos de demostración
            List<ArticuloViewModel> articulos = new List<ArticuloViewModel>();
            articulos = repoArti.GetListAllView(codRest,tipoInv, txtNum);
            return articulos;
        }


        public JsonResult ObtenerArticulos(string CodMateria, string NomMateria, decimal? Factor, decimal? Teorico, decimal? Fisico, decimal? Diferencia, string Estado /*,string CodRest, string txtNum, string txCodR*/)
        {
            string codRestS = Session["CodRe"].ToString();
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
                        articulos = articulos.Where(p => p.Conteo == true && p.Diferencia == 0).ToList();
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
        public JsonResult ActualizarArticulos(ArticuloViewModel arti)
        {
            //var productoOriginal = productos.FirstOrDefault(p => p.Id == producto.Id);
            //if (productoOriginal != null)
            //{
            //    productoOriginal.Nombre = producto.Nombre;
            //    productoOriginal.Precio = producto.Precio;
            //    productoOriginal.Cantidad = producto.Cantidad;
            //}
            // return Json(new { success = true });

            int num = 0;
            bool banderaControl= false;
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
                control.Gerente = "Christian";//falta
                control.Responsable = "Yess"; //falta
                control.Turno = "Diurno";//falta
                control.Tipo = tipoInv;
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
                //teórico
               // decimal teorico = repoHist.GetHistTeorico(arti.CodRest, arti.CodMateria);
              //  arti.Teorico = teorico;
                //diferencia
                decimal diferencia = arti.Fisico-arti.Teorico;
                arti.Diferencia = diferencia;

                //obtener el número de línea
               // int numLinea = repoInv.GetInvNumLinea(arti.CodRest, num, tipoInv);

                Inventario inv = new Inventario();
                inv.NumTF = num;
                inv.ObjArticulo = arti;
                //inv.ObjArticulo.CodRest = arti.CodRest;
                inv.FechaAudi = DateTime.Now;
                inv.Gerente = "Christian";//falta
                inv.Responsable = "Yess"; //falta
                /* inv.ObjArticulo.CodMateria = arti.CodMateria;
                inv.ObjArticulo.NomMateria = arti.NomMateria;
                inv.ObjArticulo.Factor = arti.Factor;
                inv.ObjArticulo.Teorico = arti.Teorico;
                inv.ObjArticulo.Fisico = arti.Fisico;
                inv.ObjArticulo.Diferencia = arti.Diferencia;*/
                inv.NumLinea = existe; 
                inv.CostoUni = 0;//falta
                inv.MontoVenta = 0;//falta
                inv.Tipo = tipoInv;
                inv.Turno = "Diurno";//falta
                                        
                if (existe == 0)
                {
                    //no existe la línea agregada en el inventario, se debe de insertar
                    //obtener el número de línea
                    inv.NumLinea = repoInv.GetInvNumLinea(arti.CodRest, num, tipoInv,arti.CodMateria);

                    if (repoInv.AddInvArticulo(inv) == 0)
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
        public JsonResult CerrarInventario(int tipo)//List<ArticuloViewModel> articulos
        {
            string codRestS = Session["CodRe"].ToString();
            //if (string.IsNullOrEmpty(codRestS)) { bandera = true; }
            if(tipo == 0)
            {
                TempData["Message"] = "Ocurrió un error, no se cerró el inventario";
                return Json(new { success = false });
            }
            Control control = repoControl.GetControlAbierto(codRestS, tipoInv);//verificamos si existe un inventario abierto
            if (control.NumTF > 0)
            {
                //se debe de cerrar el inventario en control
                if (repoControl.CerrarControlInventario(control.NumTF, codRestS, tipoInv, true, tipo ) == 0)
                {
                    TempData["Message"] = "Ocurrió un error, no se cerró el control del inventario";
                    return Json(new { success = false });
                }
                //si se actualizó el estado en control, continúa con las lineas en histórico
                //se debe de insertar todas las q se ingresaron el usuario y las q no a la tabla de inventario
                List<ArticuloViewModel> todosArticulos = repoArti.GetListAllView(codRestS, tipoInv, control.NumTF);
                List<Inventario> lstArticulos = new List<Inventario>();
                int contador = repoInv.GetInvNumLinea(codRestS, control.NumTF, tipoInv, "XX");
                foreach (var item in todosArticulos)
                {
                    //verifico si existe ya agregada
                    if (repoInv.ExisteArtiEnInventario(codRestS, control.NumTF,item.CodMateria, tipoInv) == 0)
                    {
                        contador++;
                        //no existe la línea agregada en el inventario, se debe de insertar
                        //obtener el número de línea              
                        Inventario inv = new Inventario();
                        inv.NumTF = control.NumTF;
                        inv.ObjArticulo = item;
                        inv.FechaAudi = DateTime.Now;
                        inv.Gerente = "Christian";//falta
                        inv.Responsable = "Yess"; //falta                      
                        //inv.NumLinea = repoInv.GetInvNumLinea(item.CodRest, control.NumTF, tipoInv,item.CodMateria);
                        inv.NumLinea =contador;
                       inv.CostoUni = 0;//falta
                        inv.MontoVenta = 0;//falta
                        inv.Tipo = tipoInv;
                        inv.Turno = "Diurno";//falta                        
                        //repoInv.AddInvArticulo(inv);//insertar linea a linea
                        lstArticulos.Add(inv);//insertar la lista
                    }
                }
                //para insertar la lista
                if (repoInv.AddInvLineaArticulo(lstArticulos) == 0)//poner si es beginTrans
                {
                    TempData["Message"] = "Ocurrió un error, cuando se agregaban al inventario";
                    return Json(new { success = false });
                }
                if (tipo == 5)//cerrar con ajuste
                {
                   //obtener todas las líneas agregadas por el usuario al inventario, que sea diferencias != 0 para ser insertadas en el histórico
                    List<ArticuloViewModel> articulos = repoInv.GetAllArtiEnInventario(codRestS,control.NumTF, tipoInv);              
                    if (repoHist.NewListHistorico(articulos) == 0)
                    {
                        repoControl.CerrarControlInventario(control.NumTF, codRestS, tipoInv, false, 7);
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