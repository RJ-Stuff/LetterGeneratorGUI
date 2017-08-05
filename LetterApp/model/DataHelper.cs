namespace LetterApp.model
{
    using System.Data;
    using System.Data.OleDb;

    // public enum DueRange
    // {
    // _91_120_Días = 0,
    // Medium = 1,
    // High = 2
    // }
    public class DataHelper
    {
        private static DataSet dataSet;

        private DataHelper() { }

        public static DataSet SampleData => dataSet ?? (dataSet = CreateDataSet());

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

            // oleDbSelectCommand1.CommandText = "SELECT * FROM Orders";
            // adapterSender.Fill(ds);
            // ds.Tables[1].TableName = "Orders";
            // ds.Tables[1].Columns.Add("FreightQuantity", typeof(SampleEnum));
            // foreach (DataRow row in ds.Tables[1].Rows)
            // {
            // double value = Convert.ToDouble(row["Freight"]);
            // if (value < 30)
            // row["FreightQuantity"] = SampleEnum.Low;
            // else if (value < 60)
            // row["FreightQuantity"] = SampleEnum.Medium;
            // else
            // {
            // row["ShipRegion"] = "";
            // row["FreightQuantity"] = SampleEnum.High;
            // }
            // }

            // oleDbSelectCommand1.CommandText = "SELECT * FROM Products";
            // adapterSender.Fill(ds);
            // ds.Tables[2].TableName = "Products";

            // oleDbSelectCommand1.CommandText = "SELECT * FROM Shippers";
            // adapterSender.Fill(ds);
            // ds.Tables[3].TableName = "Shippers";

            // oleDbSelectCommand1.CommandText = "SELECT * FROM Suppliers";
            // adapterSender.Fill(ds);
            // ds.Tables[4].TableName = "Suppliers";

            // oleDbSelectCommand1.CommandText = "SELECT * FROM [Order Details]";
            // adapterSender.Fill(ds);
            // ds.Tables[5].TableName = "OrderDetails";

            // ds.Relations.Add("Suppliers2Products", ds.Tables[4].Columns["SupplierID"], ds.Tables[2].Columns["SupplierID"]);
            // ds.Relations.Add("Products2OrderDetails", ds.Tables[2].Columns["ProductID"], ds.Tables[5].Columns["ProductID"]);
            // ds.Relations.Add("OrderDetails2Order", ds.Tables[1].Columns["OrderID"], ds.Tables[5].Columns["OrderID"]);
            return ds;
        }
    }
}