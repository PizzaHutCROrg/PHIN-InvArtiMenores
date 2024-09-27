using InventarioArtMenores.Models;
using InventarioArtMenores.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventarioArtMenores.Controllers
{
    public class RegistroMovController : Controller
    {
        RepoArticulo repoArti = new RepoArticulo();
        RepoHistorico repoHist = new RepoHistorico();
        RepoControl repoControl = new RepoControl();
        RepoTipoMov repoTipo = new RepoTipoMov();
        public static string tipoInv = "Menores";
        public ActionResult Index()
        {

            //Session["UserInvAM"] = "test";//quitar
            //Session["UserNomInvAM"] = "Yess-test";//quitar
            //Session["codResArti"] = "40"; //"36";//quitar
            //Session["desResArti"] = "Rohrmoser";//"Cartago"; //quitar

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

            //validamos si hay un inventario iniciado no permitir ingresar movimientos
            if(repoControl.SearchControl(Session["codResArti"].ToString(), tipoInv) == 1)
            {
                ViewBag.OptionsList = null;
                ViewBag.Estado = "abierto";
                TempData["Message2"] = "Tiene un inventario abierto, no se permite agregar movimientos";
            }
            else
            {
                ViewBag.Estado = "cerrado";
                ViewBag.OptionsList = repoArti.GetListAllArticulo(Session["codResArti"].ToString());//obtener la lista de string con los articulos de la tienda
               // ViewBag.OptionsTipo = repoTipo.GetTiposMovCBX();//obtener lista de string con los tipo mov activos y de seleccion, 
                ViewBag.OptionsTipo = new SelectList(repoTipo.GetTipos(), "filtrocbx", "Descripcion");
                   var model = new HistoMovEnca
                   {
                       Detalles = new List<HistoMov>() // Inicializa lista vacía
                   };
                  return View(model);
                
            }
           
            return View();
        }

        [HttpPost]
        //string txtNumDoc,string txtProv, string txtMotivo, string cbxTipoMov,   HistoMovEnca model
        public ActionResult Index(HistoMovEnca request)//string cbxArti, string txtCantidad,, string txtCosto
        {       
           // bool bandera = false;

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
            //if (string.IsNullOrEmpty(txtNumDoc) || string.IsNullOrEmpty(txtProv) || string.IsNullOrEmpty(cbxArti) || string.IsNullOrEmpty(txtCantidad) || string.IsNullOrEmpty(cbxTipoMov))
            //{
            //    TempData["Message"] = "Favor de completar los datos";
            //}
            //else
            //{
            if (ModelState.IsValid)
            {
                string[] tipoMov = request.TipoMov.Split('-');//idCategoria-TipoMov-desc
                string txtMotivo = request.Motivo;
                string txtNumDoc = request.NumDoc;
                string txtProv = request.Proveedor;
                string codRest = Session["codResArti"].ToString();
                string username = Session["UserInvAM"].ToString();


                // 2= Entrada, 3= Salida

                // Procesar la información
                 if (tipoMov[0] == "3" && string.IsNullOrEmpty(txtMotivo))//salida
                 {
                     //TempData["Message"] = "El movimiento de Salida debe tener un motivo";
                    return Json(new { success = false });
                }
                 else
                 {
                    if (string.IsNullOrEmpty(txtMotivo))
                    {
                        txtMotivo = tipoMov[2];
                    }

                    //creamos el objeto para ser enviado a la tabla de histórico
                    //encabezado
                    HistoMovEnca objEnca = new HistoMovEnca();
                    objEnca.NumDoc = txtNumDoc;
                    objEnca.Proveedor = txtProv;
                    objEnca.Motivo = txtMotivo;
                    objEnca.TipoMov = tipoMov[1];
                    objEnca.CodRest = codRest;
                    //objEnca.Detalles = obj;
                    objEnca.Username = username;
                    List<HistoMov> lts = new List<HistoMov>();//contiene los artículos agregados al detalle
                    foreach (HistoMov arti in request.Detalles)//obtenemos los artículos agregados al detalle
                    {
                        var codArti = arti.CodMateria.Split('-');//codigo-desc
                        HistoMov obj = new HistoMov();
                        obj.CodRest = codRest;
                        obj.Costo = Convert.ToDecimal(arti.Costo);
                        obj.CodMateria = codArti[0];
                        obj.DesMateria = codArti[1];
                        obj.Cantidad = Convert.ToDecimal(arti.Cantidad);
                        obj.Monto = arti.Monto;
                        if (tipoMov[0] == "2")//entrada
                        {
                            obj.Comentario = tipoMov[2];//obtenemos la descripción del tipoMov seleccionado
                            if (Convert.ToDecimal(obj.Cantidad) < 0)
                            {
                                obj.Cantidad = Convert.ToDecimal(obj.Cantidad) * -1;
                            }
                            else
                            {
                                obj.Cantidad = Convert.ToDecimal(obj.Cantidad);
                            }

                        }
                        else
                        {
                            if (tipoMov[0] == "3")//salida
                            {
                                obj.Comentario = tipoMov[2];//obtenemos la descripción del tipoMov seleccionado
                                if (Convert.ToDecimal(obj.Cantidad) > 0)
                                {
                                    obj.Cantidad = Convert.ToDecimal(obj.Cantidad) * -1;
                                }
                                else
                                {
                                    obj.Cantidad = Convert.ToDecimal(obj.Cantidad);
                                }
                            }

                        }
                        lts.Add(obj);//agregamos el objeto
                    } 

                    objEnca.Detalles = lts;//agregamos la nueva lista en el encabezado

                    if (repoHist.NewListRegMov(objEnca, username) == 0)
                    {
                        // TempData["Message"] = "Ocurrió un error registrando el movimiento";
                        return Json(new { success = false });
                    }
                   
                 }
            }
            else
            {
                //  TempData["Message"] = "Favor de completar los datos";
                return Json(new { success = false });
            }
        
            ViewBag.OptionsList = repoArti.GetListAllArticulo(Session["codResArti"].ToString());
            ViewBag.OptionsTipo = new SelectList(repoTipo.GetTipos(), "filtrocbx", "Descripcion");
            return Json(new { success = true });
        }

        // Acción para calcular el monto desde la base de datos
        //[HttpGet]
        [HttpPost]
        public ActionResult CalcularMonto(string codigoArticulo, decimal cantidad)
        {
            try
            {
                // Simulación de cálculo del monto desde la base de datos
                // Aquí puedes conectar a tu base de datos y ejecutar la lógica correspondiente
                var codArti = codigoArticulo.Split('-');//codigo-desc

                //validamos si la cantidad es negativa o no dependiendo de lo q selecciones como tipo
                //salida                                     
                if (cantidad > 0)
                {
                    cantidad = cantidad * -1;
                }
                       
                //vamos a obtener costo y monto
                List<decimal> montoCalculado = repoHist.GetMontoCalculado(codArti[0], Session["codResArti"].ToString(), cantidad);

                decimal montoCal = montoCalculado[1];
                decimal costoCal = montoCalculado[0];
                // Devolver el monto como respuesta en formato JSON
                //  return Json(new { monto = montoCalculado });
                // Devolver el monto y el costo en formato JSON
                return Json(new { costo = montoCalculado[0], monto = montoCalculado[1] });
              //  return Json(new { costo = montoCal, monto = costoCal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Manejo de errores si ocurre algún problema en la lógica
                return Json(new { error = ex.Message });
                //  return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
