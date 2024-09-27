using InventarioArtMenores.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Repos
{
    public class RepoTipoMov
    {
        string connectionString = Properties.Settings.Default.ConectionString;
        bool ambienteTest = Properties.Settings.Default.ambienteTest; //si es 1 es xq es ambiente de test
        string pathLogs = Properties.Settings.Default.PathLog;//para los logs

        public List<string> GetTiposMovCBX() //llenar el combo con los locales
        {
            List<string> lst = new List<string>();
            string sentencia = "", tablaTest = "";
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                sentencia = "select * from UX_TipoMovimiento" + tablaTest + " tipo, UX_Categoria" + tablaTest + " ca where tipo.idCategoria <> 1 and tipo.activo=1 ";
                sentencia += "and ca.activo=1 and ca.seleccion = 1 order by descripcion asc";
                SqlCommand cmd = new SqlCommand(sentencia, cnn);
                cnn.Open();
                SqlDataReader registros = cmd.ExecuteReader();
                try
                {
                    while (registros.Read())
                    {
                        //creando el objeto                                          
                        lst.Add(registros["TipoMov"].ToString().Trim() + "-" + registros["Descripcion"].ToString().Trim());
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
                    { writer.WriteLine("****Error GetTiposMovCBX()" + " fecha:" + hoy + " error: " + ex.ToString()); }
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

        public List<TipoMovimiento> GetTipos() //llenar el combo con los locales
        {
            List<TipoMovimiento> lst = new List<TipoMovimiento>();
            string sentencia = "", tablaTest = "";
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                sentencia = "select * from UX_TipoMovimiento" + tablaTest + " tipo, UX_Categoria" + tablaTest + " ca where tipo.idCategoria <> 1 and tipo.activo=1 ";
                sentencia += "and ca.activo=1 and ca.seleccion = 1 and tipo.idCategoria =ca.id  order by tipo.Descripcion asc";
                SqlCommand cmd = new SqlCommand(sentencia, cnn);
                cnn.Open();
                SqlDataReader registros = cmd.ExecuteReader();
                try
                {
                    while (registros.Read())
                    {
                        //creando el objeto
                        TipoMovimiento obj = new TipoMovimiento();
                       // obj.TipoMov = Convert.ToInt32(registros["TipoMov"]);
                       // obj.idCategoria = Convert.ToInt32(registros["idCategoria"]);
                        obj.Descripcion = registros["Descripcion"].ToString().Trim();
                        obj.filtrocbx = registros["idCategoria"].ToString().Trim() + "-" + registros["TipoMov"].ToString().Trim()+"-" + registros["Descripcion"].ToString().Trim();
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
                    { writer.WriteLine("****Error GetTiposMovCBX()" + " fecha:" + hoy + " error: " + ex.ToString()); }
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