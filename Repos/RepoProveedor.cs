using InventarioArtMenores.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Repos
{
    public class RepoProveedor
    {
        string connectionString = Properties.Settings.Default.ConectionString;
        string pathLogs = Properties.Settings.Default.PathLog;//para los logs
        bool ambienteTest = Properties.Settings.Default.ambienteTest; //si es 1 es xq es ambiente de test
        public RepoProveedor() { }

        public List<Proveedor> GetAllProveedorCbx() //llenar el combo con todos los proovedores
        {
            List<Proveedor> lst = new List<Proveedor>();
            string sentencia = "";
            string tablaTest = "";
            if (ambienteTest)
            {
                tablaTest = "Test";
            }

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                sentencia = "select Descripcion from UX_Proveedor" + tablaTest + " where activo =1 ";
                sentencia += "order by Descripcion asc";
                SqlCommand cmd = new SqlCommand(sentencia, cnn);             
                cnn.Open();
                SqlDataReader registros = cmd.ExecuteReader();
                try
                {
                    while (registros.Read())
                    {
                        //creando el objeto
                        Proveedor obj = new Proveedor();
                        obj.Descripcion = registros["Descripcion"].ToString().Trim();
                        lst.Add(obj);
                    }
                }
                catch (Exception ex)
                {
                    cnn.Dispose();
                    cnn.Close();
                    ex.ToString();
                    DateTime hoy = DateTime.Now;
                    //log
                    using (StreamWriter writer = new StreamWriter(pathLogs, true))
                    { writer.WriteLine("****Error GetAllProveedorCbx()" + " fecha:" + hoy + " error: " + ex.ToString()); }
                    //fin 
                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();
                }
            }
            return lst;
        }
    }
}