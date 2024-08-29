using InventarioArtMenores.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Repos
{
    public class RepoControl
    {
        string connectionString = Properties.Settings.Default.ConectionString;
        bool ambienteTest = Properties.Settings.Default.ambienteTest; //si es 1 es xq es ambiente de test
        string pathLogs = Properties.Settings.Default.PathLog;//para los logs
        public RepoControl() { }

        public int NewControl(Control arti)//agregamos el artículo al inventario
        {
            string sentencia = "";
            string tablaTest = "";
            int res = 0;
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    sentencia = "insert into UX_ControlInventario" + tablaTest + " (NumTF,CodRest,Gerente,Responsable,Turno,Tipo) ";              
                    sentencia += "values(@NumTF,@CodRest,@Gerente,@Responsable,@Turno,@Tipo) ";                    
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@NumTF", arti.NumTF);
                    cmd.Parameters.AddWithValue("@CodRest", arti.CodRest);
                    cmd.Parameters.AddWithValue("@Gerente", arti.Gerente);
                    cmd.Parameters.AddWithValue("@Responsable", arti.Responsable);
                    cmd.Parameters.AddWithValue("@Turno", arti.Turno);
                    cmd.Parameters.AddWithValue("@Tipo", arti.Tipo);
                  
                    cnn.Open();
                    res = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    cnn.Dispose();
                    cnn.Close();
                    //ex.ToString();
                    //log
                    DateTime hoy = DateTime.Now;
                    using (StreamWriter writer = new StreamWriter(pathLogs, true))
                    { writer.WriteLine("**Ocurrió un error NewControl() CodRest:" + arti.CodRest + " NumTF:" + arti.NumTF + " Tipo:"+arti.Tipo + " fecha:" + hoy + " ex:" + ex.ToString()); }
                    //fin 
                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();

                }
            }
            return res;
        }

        public Control GetControl(int numTF, string codRest, string tipo)//obtenemos el control del inventario
        {
            string sentencia = "";
            string tablaTest = "";
            Control obj = new Control();//objeto
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {

                try
                {
                    sentencia = "select * from UX_ControlInventario" + tablaTest + " Where NumTF=@NumTF and CodRest=@CodRest and Tipo=@Tipo ";                   
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@NumTF", numTF);
                    cmd.Parameters.AddWithValue("@CodRest", codRest);
                    cmd.Parameters.AddWithValue("@Tipo", tipo);

                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                        obj.NumTF = Convert.ToInt32(registros["NumTF"]);
                        obj.CodRest = registros["CodRest"].ToString().Trim();
                        obj.Gerente = registros["Gerente"].ToString().Trim();
                        obj.Responsable = registros["Responsable"].ToString().Trim();
                        obj.Turno = registros["Turno"].ToString().Trim();
                        obj.Tipo = registros["Tipo"].ToString().Trim();
                        obj.Cerrado = Convert.ToBoolean(registros["Cerrado"].ToString());                                             
                    }
                }
                catch (Exception ex)
                {

                    cnn.Dispose();
                    cnn.Close();
                    //ex.ToString();
                    //log
                    DateTime hoy = DateTime.Now;
                    using (StreamWriter writer = new StreamWriter(pathLogs, true))
                    { writer.WriteLine("**Ocurrió un error GetControl() CodRest:" + codRest + " NumTF:" + numTF +" Tipo:"+tipo+ " fecha:" + hoy + " ex:" + ex.ToString()); }
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

        public Control GetControlAbierto(string codRest, string tipo)//obtenemos el control del inventario abierto
        {
            string sentencia = "";
            string tablaTest = "";
            Control obj = new Control();//objeto
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {

                try
                {
                    sentencia = "select * from UX_ControlInventario" + tablaTest + " Where CodRest=@CodRest and Tipo=@Tipo and cerrado = 0";
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);                    
                    cmd.Parameters.AddWithValue("@CodRest", codRest);
                    cmd.Parameters.AddWithValue("@Tipo", tipo);

                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                        obj.NumTF = Convert.ToInt32(registros["NumTF"]);
                        obj.CodRest = registros["CodRest"].ToString().Trim();
                        obj.Gerente = registros["Gerente"].ToString().Trim();
                        obj.Responsable = registros["Responsable"].ToString().Trim();
                        obj.Turno = registros["Turno"].ToString().Trim();
                        obj.Tipo = registros["Tipo"].ToString().Trim();
                        obj.Cerrado = Convert.ToBoolean(registros["Cerrado"].ToString());
                    }
                }
                catch (Exception ex)
                {

                    cnn.Dispose();
                    cnn.Close();
                    //ex.ToString();
                    //log
                    DateTime hoy = DateTime.Now;
                    using (StreamWriter writer = new StreamWriter(pathLogs, true))
                    { writer.WriteLine("**Ocurrió un error GetControl() CodRest:" + codRest + " Tipo:" + tipo + " fecha:" + hoy + " ex:" + ex.ToString()); }
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
        public int CerrarControlInventario(int numTF, string codRest, string tipo, bool estado, int estadoMov)//actualizar la linea del artículo en el inventario
        {
            string sentencia = "", tablaTest = "";
            int res = 0;

            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    sentencia = "update UX_ControlInventario" + tablaTest + " set ";
                    sentencia += "Cerrado= @Cerrado, FechaCerrado=GetDate(), estado=@estado  ";
                    sentencia += "where NumTF=@NumTF and CodRest=@CodRest and Tipo=@Tipo ";
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@Cerrado", estado);
                    cmd.Parameters.AddWithValue("@estado", estadoMov);
                    cmd.Parameters.AddWithValue("@NumTF", numTF);
                    cmd.Parameters.AddWithValue("@CodRest", codRest);
                    cmd.Parameters.AddWithValue("@Tipo", tipo);
                    cnn.Open();
                    res = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    cnn.Dispose();
                    cnn.Close();
                    //ex.ToString();
                    //log
                    DateTime hoy = DateTime.Now;
                    using (StreamWriter writer = new StreamWriter(pathLogs, true))
                    { writer.WriteLine("**Ocurrió un error CerrarControlInventario() CodRest:" + codRest + " NumTF:" + numTF + " Tipo:" + tipo + " fecha:" + hoy + " ex:" + ex.ToString()); }
                    //fin 
                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();
                }
            }
            return res;
        }
    }
}