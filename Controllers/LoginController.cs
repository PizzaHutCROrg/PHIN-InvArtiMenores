using InventarioArtMenores.Models;
using InventarioArtMenores.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InventarioArtMenores.Controllers
{
    public class LoginController : Controller
    {
        RepoUsuario repoUsu = new RepoUsuario();
        // GET: Login
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Index(string txtCodEmp, string txtPwd)
        {
            Usuario currentEmployee = repoUsu.Login(txtCodEmp.Trim(), SHA256(txtPwd).ToUpper());
            if (currentEmployee != null)
            {
                if (!string.IsNullOrEmpty(currentEmployee.USER_NAME))
                {
                    Session["UserInvAM"]=currentEmployee.USER_NAME.Trim();
                    Session["UserNomInvAM"] = currentEmployee.FULL_NAME.Trim();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Message"] = "Error, código de Empleado y/o contraseña son incorrectos";
                }
                
            }
            else
            {
                TempData["Message"] = "Error, con el usuario ingresado. Por favor intenta nuevamente.";
            }
            return View();
        }

        public static string SHA256(string v)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(v));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }

        public async Task<ActionResult> LogOut()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Session["UserInvAM"] = null;
            Session["UserNomInvAM"] = null;
            Session["codResArti"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }

    }
}