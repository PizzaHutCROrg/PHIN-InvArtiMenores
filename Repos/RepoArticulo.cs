using InventarioArtMenores.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace InventarioArtMenores.Repos
{
    public class RepoArticulo
    {
        string connectionString = Properties.Settings.Default.ConectionString;
        bool ambienteTest = Properties.Settings.Default.ambienteTest; //si es 1 es xq es ambiente de test
        string pathLogs = Properties.Settings.Default.PathLog;//para los logs

        public List<string> GetListAllArticulo(string codrest)//cargar el cbx de los artículos
        {
            // List<Articulo> lst = new List<Articulo>();
            List<string> lst = new List<string>();
            string tablaTest = "";
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    //top 20
                    string sentencia = "select CodMateria, NomMateria from UX_AbaListaArtMenTien" + tablaTest + " where Tipo like '%Menores%' and CodRest =@CodRest order by NomMateria asc";
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@CodRest", codrest);
                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                        /*Articulo obj = new Articulo();//objeto
                        obj.CodMateria = registros["CodMateria"].ToString().Trim();
                        obj.NomMateria = registros["NomMateria"].ToString().Trim();
                        obj.CodRest = registros["CodRest"].ToString().Trim();
                        obj.NomRest = registros["NomRest"].ToString().Trim();
                        obj.Estado = Convert.ToBoolean(registros["Estado"]);
                        obj.Tipo = registros["Tipo"].ToString().Trim();
                        obj.Factor = Convert.ToDecimal(registros["Factor"]);
                        obj.Promedio = registros["Promedio"].ToString().Trim();
                        obj.Minimo = Convert.ToDecimal(registros["Minimo"]);
                        obj.Maximo = Convert.ToDecimal(registros["Maximo"]);
                        obj.Conteo = Convert.ToBoolean(registros["Conteo"]);
                        obj.CodLinea = registros["CodLinea"].ToString().Trim();
                        lst.Add(obj);*/
                        lst.Add(registros["CodMateria"].ToString().Trim()+"-"+ registros["NomMateria"].ToString().Trim());
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
                    { writer.WriteLine("**Ocurrió un error GetListAllArticulo() CodRest:" + codrest + " fecha:" + hoy + " ex:" + ex.ToString()); }
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

        public List<ArticuloViewModel> GetListAllView(string codrest, string tipo, int numTF)//cargar los artículos para la tabla
        {
            List<ArticuloViewModel> lst = new List<ArticuloViewModel>();
            string tablaTest = "";
            if (ambienteTest)
            {
                tablaTest = "Test";
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
      
                    //top 30 50
                    //string sentencia = "select top 20 * from UX_AbaListaArtMenTien" + tablaTest + " where Tipo like '%Menores%' and CodRest =@CodRest order by NomMateria asc";
                    string sentencia = "select ";
                    sentencia += "a.CodMateria, ";
                    sentencia += "a.NomMateria, ";
                    sentencia += "a.Factor,";
                    sentencia += "COALESCE(SUM(c.Cantidad), 0) AS Teorico,";
                    sentencia += "COALESCE(b.Fisico, 0) AS Fisico, ";
                    // sentencia += "COALESCE(b.Diferencia, -SUM(c.Cantidad)) AS Diferencia, ";
                    sentencia += "CASE ";
                    sentencia += "WHEN b.Diferencia IS NULL THEN COALESCE(-SUM(c.Cantidad), 0) ";
                    sentencia += "WHEN b.Diferencia = 0 THEN  COALESCE(SUM(c.Cantidad), 0)-COALESCE(b.Fisico, 0) ";//"WHEN b.Diferencia = 0 THEN  COALESCE(-SUM(c.Cantidad), 0) ";
                    sentencia += "ELSE b.Diferencia ";
                    sentencia += "END AS Diferencia, ";
                    sentencia += "a.Estado, ";
                    sentencia += "a.Conteo ";
                    sentencia += "from ";
                    sentencia += "UX_AbaListaArtMenTien"+ tablaTest + " a ";
                    sentencia += "LEFT JOIN ";
                    sentencia += "UX_OpeTFInvDiaria"+ tablaTest + " b ";
                    sentencia += "ON ";
                    sentencia += "a.CodMateria = b.CodMateria ";
                    sentencia += "and b.NumTF = @NumTF ";
                    sentencia += "and b.CodRest = a.CodRest ";
                    sentencia += "LEFT JOIN  ";
                    sentencia += "UX_HistoricoMovimiento"+ tablaTest + " c ";
                    sentencia += "on ";
                    sentencia += "a.CodMateria = c.CodMateria ";
                    sentencia += "and a.CodRest = c.CodRest ";
                    sentencia += "where ";
                    sentencia += "a.Tipo like '%"+tipo+"%' ";
                    sentencia += "and a.CodRest = @CodRest ";
                    sentencia += "group by a.CodMateria, a.NomMateria,a.Estado,a.Conteo,a.Factor,b.Fisico,b.Diferencia ";
                    sentencia += "order by a.NomMateria asc";
                    SqlCommand cmd = new SqlCommand(sentencia, cnn);
                    cmd.Parameters.AddWithValue("@NumTF", numTF);
                    cmd.Parameters.AddWithValue("@CodRest", codrest);
                    cnn.Open();
                    SqlDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                        ArticuloViewModel obj = new ArticuloViewModel();//objeto
                        obj.CodMateria = registros["CodMateria"].ToString().Trim();
                        obj.NomMateria = registros["NomMateria"].ToString().Trim();
                        obj.Estado = Convert.ToBoolean(registros["Estado"]);
                        obj.Conteo = Convert.ToBoolean(registros["Conteo"]);
                        obj.Factor = Convert.ToDecimal(registros["Factor"]);
                        obj.CodRest = codrest;// registros["CodRest"].ToString().Trim();
                        obj.Fisico = Convert.ToDecimal(registros["Fisico"]);
                        obj.Diferencia = Convert.ToDecimal(registros["Diferencia"]);
                        obj.Teorico = Convert.ToDecimal(registros["Teorico"]);
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
                    { writer.WriteLine("**Ocurrió un error GetListAllArticulo() CodRest:" + codrest + " fecha:" + hoy + " ex:" + ex.ToString()); }
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