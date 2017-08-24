namespace LetterApp.model
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;

    public class DataHelper
    {
        private DataHelper()
        {
        }

        public static int GetCount(string connectionString, string query, string filters)
        {
            var count = 0;
            
            var queryBody = File.ReadAllText(query).Replace("--CUSTOMFILTERS", filters);

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(queryBody, conn))
                {
                    cmd.CommandTimeout = 120;
                    conn.Open();
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return count;
        }

        public static DataSet GetData(string connectionString, string query, string filters)
        {
            var ds = new DataSet();
            
            var queryBody = File.ReadAllText(query).Replace("--CUSTOMFILTERS", filters);

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(queryBody, conn))
                {
                    cmd.CommandTimeout = 120;
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                        ds.Tables[0].TableName = "Clientes";
                    }
                }
            }

            return ds;
        }
    }
}