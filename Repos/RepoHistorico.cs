using InventarioArtMenores.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace InventarioArtMenores.Repos
{
    public class RepoHistorico
    {
        string connectionString = Properties.Settings.Default.ConectionString;
        bool ambienteTest = Properties.Settings.Default.ambienteTest; //si es 1 es xq es ambiente de test
        string pathLogs = Properties.Settings.Default.PathLog;//para los logs
        public RepoHistorico() { }

        public int NewHistorico(string CodRest, string CodMateria, string DesMateria, int TipoMov,int Cantidad, string Comentario)//agregamos el artículo al inventario
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
                    sentencia = "insert into UX_HistoricoMovimiento" + tablaTest + " (CodRest,CodMateria,DesMateria,TipoMov,Cantidad,Comentario) ";                  
                    sentencia += "values(@CodRest,@CodMateria,@DesMateria,@TipoMov,@Cantidad,@Comentario) ";
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@CodRest", CodRest);
                    cmd.Parameters.AddWithValue("@CodMateria", CodMateria);
                    cmd.Parameters.AddWithValue("@DesMateria", DesMateria);
                    cmd.Parameters.AddWithValue("@TipoMov", TipoMov);
                    cmd.Parameters.AddWithValue("@Cantidad", Cantidad);
                    cmd.Parameters.AddWithValue("@Comentario", Comentario);                
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
                    { writer.WriteLine("**Ocurrió un error NewHistorico() CodRest:" + CodRest + " CodMateria:" + CodMateria + " Tipo:" +TipoMov + " fecha:" + hoy + " ex:" + ex.ToString()); }
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

        public int NewListHistorico(List<ArticuloViewModel> articulos)//agregamos el artículo al inventario
        {
            string sentencia = "", comentario ="Cierre", codrest="", codMateria="";
            int numTF = 0;
            string tablaTest = "";
            int res = 0;
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {  
                cnn.Open();
                // Iniciar la transacción
                SqlTransaction transaction = cnn.BeginTransaction();
                int tipo = 0;
                try
                {
                    foreach (var item in articulos)
                    {
                        // Asignar la transacción al comando
                        //cnn.tra = transaction;
                        if (item.Diferencia > 0)
                        {
                            comentario = "Cierre-Ajuste-Positivo-NumTF " + item.NumTF;
                            tipo = 1;
                        }
                        else
                        {
                            comentario = "Cierre-Ajuste-Negativo-NumTF " + item.NumTF;
                            tipo = 2;
                        }

                        sentencia = "insert into UX_HistoricoMovimiento" + tablaTest + " (CodRest,CodMateria,DesMateria,TipoMov,Cantidad,Comentario) ";
                        sentencia += "values(@CodRest,@CodMateria,@DesMateria,@TipoMov,@Cantidad,@Comentario) ";
                        // Se pasa la transacción en el constructor del SqlCommand
                        using (SqlCommand cmd = new SqlCommand(sentencia, cnn, transaction))
                        {
                            // Aquí agregas los parámetros
                            // SqlCommand cmd = new SqlCommand(sentencia, cnn);                        
                            cmd.Parameters.AddWithValue("@CodRest", item.CodRest);
                            cmd.Parameters.AddWithValue("@CodMateria", item.CodMateria);
                            cmd.Parameters.AddWithValue("@DesMateria", item.NomMateria);
                            cmd.Parameters.AddWithValue("@TipoMov", tipo);
                            cmd.Parameters.AddWithValue("@Cantidad", item.Diferencia);
                            cmd.Parameters.AddWithValue("@Comentario", comentario);
                            codMateria = item.CodMateria;
                            codrest = item.CodRest;
                            numTF = item.NumTF;
                            // Ejecutar el comando
                            res = cmd.ExecuteNonQuery();                      
                        }                       
                    }
                    // Si todas las inserciones/actualizaciones son exitosas, se hace commit
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Si ocurre algún error, hacemos rollback
                    transaction.Rollback();
                    cnn.Dispose();
                    cnn.Close();
                    //ex.ToString();
                    //log
                    DateTime hoy = DateTime.Now;
                    using (StreamWriter writer = new StreamWriter(pathLogs, true))
                    { writer.WriteLine("**Ocurrió un error NewListHistorico() CodRest:" + codrest + " CodMateria:" + codMateria + " Tipo: Cierre-NumTF:"+numTF + " fecha:" + hoy + " ex:" + ex.ToString()); }
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
        public int GetHistTeorico(string codrest, string codMateria)//obtener el número de línea
        {

            string tablaTest = "";
            int existe = 0;
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    //top 30
                    string sentencia = "select COALESCE(SUM(Cantidad), 0) AS Cantidad from UX_HistoricoMovimiento" + tablaTest + " where CodRest =@CodRest and CodMateria =@CodMateria "; //and TipoMov <> 1
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);                  
                    cmd.Parameters.AddWithValue("@CodRest", codrest);
                    cmd.Parameters.AddWithValue("@CodMateria", codMateria);

                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                        existe = Convert.ToInt32(registros["Cantidad"]);
                    }
                }
                catch (Exception ex)
                {
                    cnn.Dispose();
                    cnn.Close();
                    // return BadRequest(ex.Message);
                    //log
                    DateTime hoy = DateTime.Now;
                    using (StreamWriter writer = new StreamWriter(pathLogs, true))
                    { writer.WriteLine("**Ocurrió un error GetHistTeorico() CodRest:" + codrest+ " CodMateria:" + codMateria + " fecha:" + hoy + " ex:" + ex.ToString()); }
                    //fin 

                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();
                }
            }

            return existe;
        }

    }
}