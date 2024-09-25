using InventarioArtMenores.Models;
using InventarioArtMenores.Models.Report;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

        public int NewRegistroMov(HistoMovEnca obj)//agregamos el movimiento al histórico
        {
            string sentencia = "";
            string tablaTest = "";
            int res = 0;
            /*if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    sentencia = "insert into UX_HistoricoMovimiento" + tablaTest + " (CodRest,CodMateria,DesMateria,TipoMov,Cantidad,Comentario,NumDoc,Proveedor,Motivo,Costo,Username) ";                  
                    sentencia += "values(@CodRest,@CodMateria,@DesMateria,@TipoMov,@Cantidad,@Comentario,@NumDoc,@Proveedor,@Motivo,@Costo,@Username) ";
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@CodRest", obj.CodRest);
                    cmd.Parameters.AddWithValue("@CodMateria", obj.Detalles.CodMateria);
                    cmd.Parameters.AddWithValue("@DesMateria", obj.Detalles.DesMateria);
                    cmd.Parameters.AddWithValue("@TipoMov", obj.TipoMov);
                    cmd.Parameters.AddWithValue("@Cantidad", obj.Detalles.Cantidad);
                    cmd.Parameters.AddWithValue("@Comentario", obj.Detalles.Comentario);                   
                    cmd.Parameters.AddWithValue("@NumDoc", obj.NumDoc);
                    cmd.Parameters.AddWithValue("@Proveedor", obj.Proveedor);
                    cmd.Parameters.AddWithValue("@Motivo", obj.Motivo);
                    cmd.Parameters.AddWithValue("@Costo", obj.Detalles.Costo);
                    cmd.Parameters.AddWithValue("@Username", obj.Username);
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
                    { writer.WriteLine("**Ocurrió un error NewRegistroMov() CodRest:" + obj.CodRest + " CodMateria:" + obj.Detalles.CodMateria + " Tipo:" +obj.TipoMov + " fecha:" + hoy + " ex:" + ex.ToString()); }
                    //fin 
                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();

                }
            }*/
            return res;
        }

        public int NewListRegMov(HistoMovEnca articulos, string username)//agregamos los artículos que vienen en el registro de movimiento (encabezado y detalle)
        {
            string sentencia = "", codrest = "", codMateria = "";           
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
                try
                {
                    foreach (var item in articulos.Detalles)
                    {
                        // Asignar la transacción al comando
                        //cnn.tra = transaction;                     
                        sentencia = "insert into UX_HistoricoMovimiento" + tablaTest + " (CodRest,CodMateria,DesMateria,TipoMov,Cantidad,Comentario,Username,NumDoc,Proveedor,Motivo,Costo,Monto) ";
                        sentencia += "values(@CodRest,@CodMateria,@DesMateria,@TipoMov,@Cantidad,@Comentario,@Username,@NumDoc,@Proveedor,@Motivo,@Costo,@Monto) ";
                        // Se pasa la transacción en el constructor del SqlCommand
                        using (SqlCommand cmd = new SqlCommand(sentencia, cnn, transaction))
                        {
                            // Aquí agregas los parámetros
                            // SqlCommand cmd = new SqlCommand(sentencia, cnn);                        
                            cmd.Parameters.AddWithValue("@CodRest", item.CodRest);
                            cmd.Parameters.AddWithValue("@CodMateria", item.CodMateria);
                            cmd.Parameters.AddWithValue("@DesMateria", item.DesMateria);
                            cmd.Parameters.AddWithValue("@TipoMov", articulos.TipoMov);
                            cmd.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                            cmd.Parameters.AddWithValue("@Comentario", item.Comentario);
                            cmd.Parameters.AddWithValue("@Username", articulos.Username);
                            cmd.Parameters.AddWithValue("@NumDoc", articulos.NumDoc);

                            cmd.Parameters.AddWithValue("@Proveedor", articulos.Proveedor);
                            cmd.Parameters.AddWithValue("@Motivo", articulos.Motivo);
                            cmd.Parameters.AddWithValue("@Costo", item.Costo);
                            cmd.Parameters.AddWithValue("@Monto", item.Monto);

                            codMateria = item.CodMateria;
                            codrest = item.CodRest;    
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
                    { writer.WriteLine("**Ocurrió un error NewListHistorico() CodRest:" + codrest + " CodMateria:" + codMateria + " NumDoc:" + articulos.NumDoc +" TipoMov:"+articulos.TipoMov+" fecha:" + hoy + " ex:" + ex.ToString()); }
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

        public int NewListHistorico(List<ArticuloViewModel> articulos, string username)//agregamos el artículo del inventario al histórico en el cierre con ajuste
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

                        sentencia = "insert into UX_HistoricoMovimiento" + tablaTest + " (CodRest,CodMateria,DesMateria,TipoMov,Cantidad,Comentario,Username,NumDoc,Costo,Monto) ";
                        sentencia += "values(@CodRest,@CodMateria,@DesMateria,@TipoMov,@Cantidad,@Comentario,@Username,@NumDoc,@Costo,@Monto) ";
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
                            cmd.Parameters.AddWithValue("@Username", username);
                            cmd.Parameters.AddWithValue("@NumDoc", item.NumTF);
                            cmd.Parameters.AddWithValue("@Costo", item.CostoUni);
                            cmd.Parameters.AddWithValue("@Monto", item.MontoVenta);
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

        public List<decimal> GetMontoCalculado(string codMateria, string codRest, decimal cantidad)//obtener el monto calculado por la función
        {
            //decimal monto = 0;
            string tablaTest = "";
            List<decimal> resultados = new List<decimal>();
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {                 
                    string sentencia = "Select * from fnInvTraeCostoMonto" + tablaTest + " (@CodRest,@CodMateria,@Cantidad) ";
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@CodRest", codRest);
                    cmd.Parameters.AddWithValue("@CodMateria", codMateria);
                    cmd.Parameters.AddWithValue("@Cantidad", cantidad);

                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                        // existe.Add(Convert.ToDecimal(registros["Costo"]));
                        // existe.Add(Convert.ToDecimal(registros["Monto"]));

                        decimal costo = registros.GetDecimal(0); // Obtener el valor de 'costo'
                        decimal monto = registros.GetDecimal(1); // Obtener el valor de 'monto'
                       
                        // Añadir los valores a la lista                       
                        resultados.Add(costo);
                        resultados.Add(monto);
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
                    { writer.WriteLine("**Ocurrió un error GetMonto() CodRest:" + codRest + " CodMateria:" + codMateria + " cantidad:" + cantidad+ " fecha:" + hoy + " ex:" + ex.ToString()); }
                    //fin 

                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();
                }
            }


            return resultados;
        }

        public List<KardexViewModel> KarGetHistArticulo(string codrest, string codMateria)//obtener el número de línea
        {

            string tablaTest = "";            
            List<KardexViewModel> lst = new List<KardexViewModel>();
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    //top 30
                    string sentencia = "select CodMateria, DesMateria, Cantidad, isnull(Costo, 0) Costo, isnull(Monto, 0) monto, Comentario, NumDoc, ";
                    sentencia += "Proveedor, Motivo, SUM(Cantidad) OVER (ORDER BY FechaReg ROWS UNBOUNDED PRECEDING) AS AcumuladoCantidad, FechaReg ";
                    sentencia += "from UX_HistoricoMovimiento" + tablaTest + " where CodRest =@CodRest and CodMateria =@CodMateria order by FechaReg desc"; 
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@CodRest", codrest);
                    cmd.Parameters.AddWithValue("@CodMateria", codMateria);

                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                        // existe = Convert.ToInt32(registros["Cantidad"]);
                        KardexViewModel obj = new KardexViewModel();
                        obj.CodMateria = registros["CodMateria"].ToString().Trim();
                        obj.DesMateria = registros["DesMateria"].ToString().Trim();
                        obj.Comentario = registros["Comentario"].ToString().Trim();
                        obj.CodRest = codrest;
                        obj.Monto = Convert.ToDecimal(registros["Monto"]);
                        obj.Costo = Convert.ToDecimal(registros["Costo"]);
                        obj.Cantidad = Convert.ToDecimal(registros["Cantidad"]);
                        obj.NumDoc = registros["NumDoc"].ToString().Trim();
                        obj.Proveedor = registros["Proveedor"].ToString().Trim();
                        obj.Motivo = registros["Motivo"].ToString().Trim();
                        obj.Acumulado = Convert.ToDecimal(registros["AcumuladoCantidad"]);
                        obj.FechaReg = Convert.ToDateTime(registros["FechaReg"]);
                        lst.Add(obj);
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
                    { writer.WriteLine("**Ocurrió un error KarGetHistArticulo() CodRest:" + codrest + " CodMateria:" + codMateria + " fecha:" + hoy + " ex:" + ex.ToString()); }
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
        public List<SaldosViewModel> SalGetHistArticulo(string codrest)//obtener el número de línea
        {

            string tablaTest = "";
            List<SaldosViewModel> lst = new List<SaldosViewModel>();
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    //top 30
                    string sentencia = "Select * from fnInvReporteInventario" + tablaTest + " (@CodRest) ";
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@CodRest", codrest);
                  
                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {                       
                        SaldosViewModel obj = new SaldosViewModel();
                        obj.CodMateria = registros.GetString(0);
                        obj.NomMateria = registros.GetString (1);
                        obj.Estado =registros.GetBoolean(2);
                        obj.Minimo = registros.GetDecimal(3);
                        obj.Maximo = registros.GetDecimal(4);
                        obj.Tipo = registros.GetString(5);
                        obj.Cantidad = registros.GetDecimal(6);
                        obj.Costo = registros.GetDecimal(7);
                        obj.Monto = registros.GetDecimal(8);
                        obj.UC_Cantidad = registros.GetDecimal(9);
                        obj.UC_Costo = registros.GetDecimal(10);
                        obj.UC_Monto = registros.GetDecimal(11);
                        obj.UC_Fecha = registros.GetDateTime(12);
                        obj.TipoNivelInv = registros.GetString(13);                                                                     
                        obj.CodRest = codrest;
                        lst.Add(obj);
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
                    { writer.WriteLine("**Ocurrió un error SalGetHistArticulo() CodRest:" + codrest + " fecha:" + hoy + " ex:" + ex.ToString()); }
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