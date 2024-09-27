using InventarioArtMenores.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Repos
{
    public class RepoUsuario
    {
       // string connectionString = Properties.Settings.Default.ConectionStringLogin;//volver a poner Producción
        string connectionString = Properties.Settings.Default.ConectionStringLoginTest;//quitar test
        bool ambienteTest = Properties.Settings.Default.ambienteTest; //si es 1 es xq es ambiente de test      
        string pathLogs = Properties.Settings.Default.PathLog;//para los logs

        public Usuario Login(string cod, string pwd)//buscar el usuario, acceso
        {
            Usuario obj = new Usuario();
            string sentencia = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    sentencia = "select USER_NAME, FULL_NAME from Employees where USER_NAME=@USER_NAME and PASSWORD=@PASSWORD and STATUS_INV_ART_MEN=1";
                    SqlCommand cmd2 = new SqlCommand(sentencia, cnn);
                    cmd2.Parameters.AddWithValue("@USER_NAME", cod);
                    cmd2.Parameters.AddWithValue("@PASSWORD", pwd);
                    cnn.Open();
                    SqlDataReader registros = cmd2.ExecuteReader();

                    while (registros.Read())
                    {
                        obj.USER_NAME = registros["USER_NAME"].ToString().Trim();
                        obj.FULL_NAME = registros["FULL_NAME"].ToString().Trim();                       
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
                    { writer.WriteLine("****Error Usuario Login() USER_NAME:" + cod + " fecha:" + hoy + " error: " + ex.ToString()); }
                    //fin
                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();
                }
            }
            return obj;
        }

        public Usuario Get(string cod)// obtener los datos del usuario logueado
        {
            Usuario obj = new Usuario();
            string sentencia = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    sentencia = "select USER_NAME, FULL_NAME from Employees where USER_NAME=@USER_NAME";
                    SqlCommand cmd2 = new SqlCommand(sentencia, cnn);
                    cmd2.Parameters.AddWithValue("@USER_NAME", cod);
                   
                    cnn.Open();
                    SqlDataReader registros = cmd2.ExecuteReader();

                    while (registros.Read())
                    {
                        obj.USER_NAME = registros["USER_NAME"].ToString().Trim();
                        obj.FULL_NAME = registros["FULL_NAME"].ToString().Trim();

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
                    { writer.WriteLine("****Error Usuario Get() USER_NAME:" + cod + " fecha:" + hoy + " error: " + ex.ToString()); }
                    //fin
                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();
                }
            }
            return obj;
        }

        public List<string> GetUserLocal(string cod)// obtener los locales asociados al usuario
        {
            List<string> lts = new List<string>();
            string sentencia = "";
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    sentencia = "SELECT POINT_SALE FROM EmployeesXRestaurant where USER_NAME=@USER_NAME and STATUS_INV_ART_MEN =1";
                    SqlCommand cmd2 = new SqlCommand(sentencia, cnn);
                    cmd2.Parameters.AddWithValue("@USER_NAME", cod);

                    cnn.Open();
                    SqlDataReader registros = cmd2.ExecuteReader();

                    while (registros.Read())
                    {
                        string num = registros["POINT_SALE"].ToString().Trim();
                        if (num.Length == 1)
                        {
                            num = "0" + num;
                        }
                        lts.Add(num); 
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
                    { writer.WriteLine("****Error Usuario Get() USER_NAME:" + cod + " fecha:" + hoy + " error: " + ex.ToString()); }
                    //fin
                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();
                }
            }
            return lts;
        }


    }
}