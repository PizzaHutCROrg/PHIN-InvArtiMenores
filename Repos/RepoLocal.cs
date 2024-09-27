using InventarioArtMenores.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Repos
{
    public class RepoLocal
    {
        string connectionString = Properties.Settings.Default.ConectionString;
        string pathLogs = Properties.Settings.Default.PathLog;//para los logs

        public List<Local> GetAllLocalesCBX(List<string> ltsCodRest) //llenar el combo con los locales
        {
            List<Local> lst = new List<Local>();
            string sentencia = "", filtro="", lista ="";

            for(int i = 0; i < ltsCodRest.Count; i++)
            {
                if(i == ltsCodRest.Count - 1)
                {
                    lista += "'" + ltsCodRest[i] + "'";
                }
                else
                {
                    lista += "'" + ltsCodRest[i] + "',";
                }
               
            }
            filtro = " and CodRest in ("+ lista+")";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                sentencia = "select CodRest, Descripcion from GPizzaHut.dbo.Locales where activo =1"+filtro;
                sentencia += "order by CodRest asc";
                SqlCommand cmd = new SqlCommand(sentencia, cnn);
                cnn.Open();
                SqlDataReader registros = cmd.ExecuteReader();
                try
                {
                    while (registros.Read())
                    {
                        //creando el objeto
                        Local obj = new Local();
                        obj.CodRest = registros["CodRest"].ToString().Trim();
                        obj.Descripcion = registros["Descripcion"].ToString().Trim();
                        obj.filtrocbx = registros["CodRest"].ToString().Trim() + "-" + registros["Descripcion"].ToString().Trim();
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
                    { writer.WriteLine("****Error GetAllLocalesCBX()" + " fecha:" + hoy + " error: " + ex.ToString()); }
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

        public string GetDescripcion() //llenar el combo con los locales
        {
            List<Local> lst = new List<Local>();
            string sentencia = "";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                sentencia = "select Descripcion from GPizzaHut.dbo.Locales where activo =1 ";
                sentencia += "order by CodRest asc";
                SqlCommand cmd = new SqlCommand(sentencia, cnn);
                cnn.Open();
                SqlDataReader registros = cmd.ExecuteReader();
                try
                {
                    while (registros.Read())
                    {
                        //creando el objeto
                        Local obj = new Local();
                        obj.CodRest = registros["CodRest"].ToString().Trim();
                        obj.Descripcion = registros["Descripcion"].ToString().Trim();
                        obj.filtrocbx = registros["CodRest"].ToString().Trim() + "-" + registros["Descripcion"].ToString().Trim();
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
                    { writer.WriteLine("****Error GetAllLocalesCBX()" + " fecha:" + hoy + " error: " + ex.ToString()); }
                    //fin 
                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();
                }
            }
            return "";
        }

    }
}