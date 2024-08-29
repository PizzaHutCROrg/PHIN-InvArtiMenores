using InventarioArtMenores.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Repos
{
    public class RepoInventario
    {
        string connectionString = Properties.Settings.Default.ConectionString;
        bool ambienteTest = Properties.Settings.Default.ambienteTest; //si es 1 es xq es ambiente de test
        string pathLogs = Properties.Settings.Default.PathLog;//para los logs

        public int ExisteArtiEnInventario(string codrest, int numtf, string codmateria, string tipo)//verificar si existe la linea
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
                    string sentencia = "select NumLinea from UX_OpeTFInvDiaria" + tablaTest + " where CodRest =@CodRest and NumTF = @NumTF and CodMateria=@CodMateria";//TipoUso=@TipoUso and
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    //cmd.Parameters.AddWithValue("@TipoUso", tipo);
                    cmd.Parameters.AddWithValue("@CodRest", codrest);
                    cmd.Parameters.AddWithValue("@NumTF", numtf);
                    cmd.Parameters.AddWithValue("@CodMateria", codmateria);
                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                       existe = Convert.ToInt32(registros["NumLinea"]);
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
                    { writer.WriteLine("**Ocurrió un error ExisteArtiEnInventario() CodRest:" + codrest +" CodMateria:"+codmateria+ " NumTF:"+numtf +"fecha:" + hoy + " ex:" + ex.ToString()); }
                    //fin 

                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();
                }
            }
            ////log
            //DateTime hoy2 = DateTime.Now;
            //using (StreamWriter writer = new StreamWriter(pathLogs, true))
            //{ writer.WriteLine("**ENCONTRÓ** =>"+ existe +"  -ExisteArtiEnInventario() CodRest:" + codrest + " CodMateria:" + codmateria + " NumTF:" + numtf + "fecha:" + hoy2); }
            ////fin 

            return existe;
        }

        public int AddInvArticulo(Inventario arti)//agregamos el artículo al inventario
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
                    sentencia = "insert into UX_OpeTFInvDiaria" + tablaTest + " (NumTF,CodRest,FechaRegis,FechaAudi,Gerente,Responsable,CodMateria,DesMateria,UnidMedInv, ";
                    sentencia += "Teorico,Fisico,Diferencia,NumLinea,CostoUni,TipoUso,MontoVenta,FechaRegisReal,Turno) ";
                    sentencia += "values(@NumTF,@CodRest,getDate(),getDate(),@Gerente,@Responsable,@CodMateria,@DesMateria,@UnidMedInv, ";
                    sentencia += "@Teorico,@Fisico,@Diferencia,@NumLinea,@CostoUni,'Menores',@MontoVenta,getDate(),@Turno)";
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@NumTF", arti.NumTF);
                    cmd.Parameters.AddWithValue("@CodRest", arti.ObjArticulo.CodRest);
                   // cmd.Parameters.AddWithValue("@FechaAudi", arti.FechaAudi);
                    cmd.Parameters.AddWithValue("@Gerente", arti.Gerente);
                    cmd.Parameters.AddWithValue("@Responsable", arti.Responsable);
                    cmd.Parameters.AddWithValue("@CodMateria", arti.ObjArticulo.CodMateria);
                    cmd.Parameters.AddWithValue("@DesMateria", arti.ObjArticulo.NomMateria);
                    cmd.Parameters.AddWithValue("@UnidMedInv", arti.ObjArticulo.Factor);
                    cmd.Parameters.AddWithValue("@Teorico", arti.ObjArticulo.Teorico);
                    cmd.Parameters.AddWithValue("@Fisico", arti.ObjArticulo.Fisico);
                    cmd.Parameters.AddWithValue("@Diferencia", arti.ObjArticulo.Diferencia);
                    cmd.Parameters.AddWithValue("@NumLinea", arti.NumLinea);
                    cmd.Parameters.AddWithValue("@CostoUni", arti.CostoUni);
                    cmd.Parameters.AddWithValue("@MontoVenta", arti.MontoVenta);
                    cmd.Parameters.AddWithValue("@Turno", arti.Turno);                  
                    cnn.Open();
                    res = cmd.ExecuteNonQuery();

                   string sentencia2 = "insert into UX_OpeTFInvDiaria" + tablaTest + " (NumTF,CodRest,FechaRegis,FechaAudi,Gerente,Responsable,CodMateria,DesMateria,UnidMedInv, ";
                    sentencia2 += "Teorico,Fisico,Diferencia,NumLinea,CostoUni,TipoUso,MontoVenta,FechaRegisReal,Turno) ";
                    sentencia2 += "values("+ arti.NumTF + ",'" +arti.ObjArticulo.CodRest + "',getDate(),getDate(),'"+ arti.Gerente + "','"+arti.Responsable+"','"+arti.ObjArticulo.CodMateria+"','"+arti.ObjArticulo.NomMateria+"',"+arti.ObjArticulo.Factor+", ";
                    sentencia2 += arti.ObjArticulo.Teorico+","+arti.ObjArticulo.Fisico+","+ arti.ObjArticulo.Diferencia+","+arti.NumLinea+","+ arti.CostoUni+",'Menores',"+arti.MontoVenta+",getDate(),'"+arti.Turno+"')";
                    using (StreamWriter writer = new StreamWriter(pathLogs, true))
                    { writer.WriteLine(sentencia2); }
                }
                catch (Exception ex)
                {

                    cnn.Dispose();
                    cnn.Close();
                    //ex.ToString();
                    //log
                    DateTime hoy = DateTime.Now;
                    using (StreamWriter writer = new StreamWriter(pathLogs, true))
                    { writer.WriteLine("**Ocurrió un error AddArticulo() CodRest:" + arti.ObjArticulo.CodRest + " Codigo:" + arti.ObjArticulo.CodMateria+" NumTF:"+ arti.NumTF + " fecha:" + hoy + " ex:" + ex.ToString()); }
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

        public int AddInvLineaArticulo(List<Inventario> articulos)//agregamos el artículo al inventario
        {
            string sentencia = "", codMateria ="", codrest="";
            string tablaTest = "";
            int res = 0, numTF=0;
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
                        sentencia = "insert into UX_OpeTFInvDiaria" + tablaTest + " (NumTF,CodRest,FechaRegis,FechaAudi,Gerente,Responsable,CodMateria,DesMateria,UnidMedInv, ";
                        sentencia += "Teorico,Fisico,Diferencia,NumLinea,CostoUni,TipoUso,MontoVenta,FechaRegisReal,Turno) ";
                        sentencia += "values(@NumTF,@CodRest,getDate(),@FechaAudi,@Gerente,@Responsable,@CodMateria,@DesMateria,@UnidMedInv, ";
                        sentencia += "@Teorico,@Fisico,@Diferencia,@NumLinea,@CostoUni,'Menores',@MontoVenta,getDate(),@Turno)";
                        // Se pasa la transacción en el constructor del SqlCommand
                        using (SqlCommand cmd = new SqlCommand(sentencia, cnn, transaction))
                        {
                            // Aquí agregas los parámetros
                            // SqlCommand cmd = new SqlCommand(sentencia, cnn);                        
                            cmd.Parameters.AddWithValue("@NumTF", item.NumTF);
                            cmd.Parameters.AddWithValue("@CodRest", item.ObjArticulo.CodRest);
                            cmd.Parameters.AddWithValue("@FechaAudi", item.FechaAudi);
                            cmd.Parameters.AddWithValue("@Gerente", item.Gerente);
                            cmd.Parameters.AddWithValue("@Responsable", item.Responsable);
                            cmd.Parameters.AddWithValue("@CodMateria", item.ObjArticulo.CodMateria);
                            cmd.Parameters.AddWithValue("@DesMateria", item.ObjArticulo.NomMateria);
                            cmd.Parameters.AddWithValue("@UnidMedInv", item.ObjArticulo.Factor);
                            cmd.Parameters.AddWithValue("@Teorico", item.ObjArticulo.Teorico);
                            cmd.Parameters.AddWithValue("@Fisico", item.ObjArticulo.Fisico);
                            cmd.Parameters.AddWithValue("@Diferencia", item.ObjArticulo.Diferencia);
                            cmd.Parameters.AddWithValue("@NumLinea", item.NumLinea);
                            cmd.Parameters.AddWithValue("@CostoUni", item.CostoUni);
                            cmd.Parameters.AddWithValue("@MontoVenta", item.MontoVenta);
                            cmd.Parameters.AddWithValue("@Turno", item.Turno);
                            codMateria = item.ObjArticulo.CodMateria;
                            codrest = item.ObjArticulo.CodRest;
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

                    cnn.Dispose();
                    cnn.Close();
                    //ex.ToString();
                    //log
                    DateTime hoy = DateTime.Now;
                    using (StreamWriter writer = new StreamWriter(pathLogs, true))
                    { writer.WriteLine("**Ocurrió un error AddInvLineaArticulo() CodRest:" + codrest + " Codigo:" + codMateria + " NumTF:" + numTF + " fecha:" + hoy + " ex:" + ex.ToString()); }
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

        public int UpdateInvArticulo(Inventario arti)//actualizar la linea del artículo en el inventario
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
                    sentencia = "update UX_OpeTFInvDiaria" + tablaTest + " set ";
                    sentencia += "Teorico = @Teorico, Fisico = @Fisico, Diferencia=@Diferencia ";
                    sentencia += "where NumTF=@NumTF and CodRest=@CodRest and CodMateria=@CodMateria and TipoUso= 'Menores' ";
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@Teorico", arti.ObjArticulo.Teorico);
                    cmd.Parameters.AddWithValue("@Fisico", arti.ObjArticulo.Fisico);
                    cmd.Parameters.AddWithValue("@Diferencia", arti.ObjArticulo.Diferencia);
                    cmd.Parameters.AddWithValue("@NumTF", arti.NumTF);
                    cmd.Parameters.AddWithValue("@CodRest", arti.ObjArticulo.CodRest);
                    cmd.Parameters.AddWithValue("@CodMateria", arti.ObjArticulo.CodMateria);
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
                    { writer.WriteLine("**Ocurrió un error UpdateInvArticulo() CodRest" + arti.ObjArticulo.CodRest + " código:" + arti.ObjArticulo.CodMateria + " fecha:" + hoy + " ex:" + ex.ToString()); }
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

        public int GetInvNumTF(string codrest, string tipo)//obtener el numTF que sigue
        {

            string tablaTest = "";
            int num = 0;
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    //top 30 'Menores'
                    string sentencia = "select max(numtf)+1 num from UX_OpeTFInvDiaria" + tablaTest + " where CodRest =@CodRest"; // and TipoUso=@TipoUso
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@CodRest", codrest);
                    //cmd.Parameters.AddWithValue("@TipoUso", tipo);
                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                        num = Convert.ToInt32(registros["num"]);
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
                    { writer.WriteLine("**Ocurrió un error GetInvNumTF() CodRest:" + codrest + " fecha:" + hoy + " ex:" + ex.ToString()); }
                    //fin 
                }
                finally
                {
                    cnn.Dispose();
                    cnn.Close();
                }
            }

            return num;
        }
        public int GetInvNumLinea(string codrest, int numtf, string tipo, string codmateria)//obtener el número de línea
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
                    //top 30  max(NumLinea)+1 NumLinea
                    string sentencia = "select COALESCE(count(codmateria)+1, 1) NumLinea from UX_OpeTFInvDiaria" + tablaTest + " where  CodRest =@CodRest and NumTF = @NumTF"; //TipoUso=@TipoUso and
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    //cmd.Parameters.AddWithValue("@TipoUso", tipo);
                    cmd.Parameters.AddWithValue("@CodRest", codrest);
                    cmd.Parameters.AddWithValue("@NumTF", numtf);
                   
                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                        existe = Convert.ToInt32(registros["NumLinea"]);
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
                    { writer.WriteLine("**Ocurrió un error GetInvNumLinea() CodRest:" + codrest +" NumTF:"+numtf+" tipo:"+tipo+" codmateria:"+codmateria+ " fecha:" + hoy + " ex:" + ex.ToString()); }
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
        public List<ArticuloViewModel> GetAllArtiEnInventario(string codrest, int numtf, string tipo)//obtener todas las líneas agregadas por el usuario al inventario
        {

            string tablaTest = "";
            List<ArticuloViewModel> lst = new List<ArticuloViewModel>();
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    //top 30
                    string sentencia = "select codRest,CodMateria,DesMateria as NomMateria,Fisico,Teorico,Diferencia from UX_OpeTFInvDiaria" + tablaTest + " where TipoUso=@TipoUso and CodRest =@CodRest and NumTF = @NumTF and Diferencia <> 0 order by DesMateria asc ";
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@TipoUso", tipo);
                    cmd.Parameters.AddWithValue("@CodRest", codrest);
                    cmd.Parameters.AddWithValue("@NumTF", numtf);
                   
                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                       // Inventario inv = new Inventario();
                        ArticuloViewModel arti = new ArticuloViewModel();
                        arti.NumTF = numtf;
                        arti.CodMateria = registros["CodMateria"].ToString().Trim();
                        arti.NomMateria = registros["NomMateria"].ToString().Trim();
                        arti.CodRest = registros["CodRest"].ToString().Trim();                     
                        arti.Fisico = Convert.ToDecimal(registros["Fisico"]);
                        arti.Teorico = Convert.ToDecimal(registros["Teorico"]);
                        arti.Diferencia = Convert.ToDecimal(registros["Diferencia"]);
                        /* inv.NumTF= numtf;
                         inv.NumLinea = Convert.ToInt32(registros["NumLinea"]);
                         ArticuloViewModel arti = new ArticuloViewModel();
                         arti.NumTF= numtf;
                         arti.CodMateria = registros["CodMateria"].ToString().Trim();
                         arti.NomMateria = registros["NomMateria"].ToString().Trim();
                         arti.CodRest = registros["CodRest"].ToString().Trim();                       
                         inv.ObjArticulo = arti;
                         inv.MontoVenta = Convert.ToDecimal(registros["Diferencia"]);
                         inv.CostoUni = Convert.ToDecimal(registros["Diferencia"]);
                         inv.Tipo = registros["CodRest"].ToString().Trim();
                         inv.Turno = registros["CodRest"].ToString().Trim();*/
                        lst.Add(arti);

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
                    { writer.WriteLine("**Ocurrió un error GetAllArtiEnInventario() CodRest:" + codrest + " fecha:" + hoy + " ex:" + ex.ToString()); }
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