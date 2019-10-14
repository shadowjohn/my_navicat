using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utility;
using System.Data;
using System.Data.SqlClient;
namespace my_navicat.lib
{
    class my_mssql
    {
        myinclude my = new myinclude();
        public SqlConnection MCT = null;
        public SqlCommand MC = null;
        public SqlParameter PA = null;

        public void setConn(string connection)
        {
            MCT = new SqlConnection(connection);
        }
        public void setTimeout(int timeout)
        {
            //second
            MC.CommandTimeout = timeout;
        }
        public void open()
        {
            MCT.Open();
        }
        public void close()
        {
            MCT.Close();
        }
        public DataTable selectSQL_SAFE(string SQL)
        {
            return selectSQL_SAFE(SQL, new Dictionary<string, object>());
        }
        public DataTable selectSQL_SAFE(string SQL, Dictionary<string, object> key_value)
        {
            DataTable output = new DataTable();
            MC = new SqlCommand(SQL, MCT);
            //List<MySqlParameter> PAList = new List<MySqlParameter>();
            foreach (var key in key_value.Keys)
            {
                PA = new SqlParameter("@" + key, key_value[key]);
                MC.Parameters.Add(PA);
            }
            output.Load(MC.ExecuteReader());
            MC.Dispose();
            return output;
        }
        public Dictionary<string, string> execSQL_SAFE(string SQL, Dictionary<string, object> m)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            try
            {
                foreach (var key in m.Keys)
                {
                    PA = new SqlParameter("@" + key, m[key]);
                    MC.Parameters.Add(PA);
                }
                MC.ExecuteScalar();
                output["status"] = "OK";
                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output["status"] = "NO";
                output["reason"] = ex.Message;
                return output;
            }
        }
        public Dictionary<string, object> insertSQL_SAFE(string table, Dictionary<string, object> m)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            try
            {
                int LAST_ID = -1;
                List<string> keys = new List<string>();
                List<string> qa = new List<string>();
                foreach (var key in m.Keys)
                {
                    keys.Add(key);
                    qa.Add("@" + key);
                }
                string SQL = @"
                INSERT INTO [" + table + @"]" +
                    @"(["
                        + my.implode("],[", keys) +
                    @"])
                VALUES("
                        + my.implode(",", qa) +
                    @")";
                MC = new SqlCommand(SQL, MCT);
                foreach (var key in m.Keys)
                {
                    PA = new SqlParameter("@" + key, m[key]);
                    MC.Parameters.Add(PA);
                }
                LAST_ID = Convert.ToInt32(MC.ExecuteScalar());
                output["status"] = "OK";
                output["LAST_ID"] = LAST_ID;
                return output;
            }
            catch (Exception ex)
            {
                output["status"] = "NO";
                output["reason"] = ex.Message;
                return output;
            }
        }
        public Dictionary<string, object> updateSQL_SAFE(string table, Dictionary<string, object> m, string whereSQL, Dictionary<string, object> wm)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            try
            {
                whereSQL = whereSQL.Replace("@", "@");
                List<string> fields = new List<string>();
                foreach (var key in m.Keys)
                {
                    fields.Add("[" + key + "]=@" + key);
                }
                string SQL = @"
                UPDATE [" + table + @"] SET " +
                     my.implode(",", fields) +
                @"
                    WHERE 
                        1=1
                        " + whereSQL + @"
                ";
                MC = new SqlCommand(SQL, MCT);
                foreach (var key in m.Keys)
                {
                    PA = new SqlParameter("@" + key, m[key]);
                    MC.Parameters.Add(PA);
                }
                foreach (var key in wm.Keys)
                {
                    PA = new SqlParameter("@" + key, wm[key]);
                    MC.Parameters.Add(PA);
                }
                MC.ExecuteScalar();
                output["status"] = "OK";
                return output;
            }
            catch (Exception ex)
            {
                output["status"] = "NO";
                output["reason"] = ex.Message;
                return output;
            }
        }

    }
}
