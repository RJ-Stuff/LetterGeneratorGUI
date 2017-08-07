namespace LetterApp.model
{
    using System.Data;
    using System.Data.OleDb;

    public class DataHelper
    {
        private static DataSet dataSet;

        private DataHelper()
        {
        }

        public static DataSet SampleData => CreateDataSet();//dataSet ?? (dataSet = CreateDataSet());

        private static DataSet CreateDataSet()
        {
            var ds = new DataSet();

            IDbConnection connection = new OleDbConnection
            {
                ConnectionString = @"Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Registry Path=;Jet OLEDB:Database Locking Mode=0;Jet OLEDB:Database Password=;Data Source=""clients.mdb"";Password=;Jet OLEDB:Engine Type=3;Jet OLEDB:Global Bulk Transactions=1;Provider=""Microsoft.Jet.OLEDB.4.0"";Jet OLEDB:System database=;Jet OLEDB:SFP=False;Extended Properties=;Mode=Share Deny None;Jet OLEDB:New Database Password=;Jet OLEDB:Create System Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;User ID=Admin;Jet OLEDB:Encrypt Database=False"
            };
            IDbDataAdapter adapterSender = new OleDbDataAdapter();
            IDbCommand oleDbSelectCommand1 = new OleDbCommand();
            oleDbSelectCommand1.Connection = connection;
            adapterSender.SelectCommand = oleDbSelectCommand1;

            oleDbSelectCommand1.CommandText = "SELECT * FROM Client";
            adapterSender.Fill(ds);
            ds.Tables[0].TableName = "Clientes";
            return ds;
        }
    }
}