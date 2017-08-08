namespace LetterApp.view
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using LetterCore.Letters;

    public class ViewUtils
    {
        public static IEnumerable<List<DataRowView>> GroupBy(IEnumerable<DataRowView> data, string key) =>
            data.GroupBy(row => row[key], (k, group) => @group.ToList());

        public static Client GetClient(DataRowView rowView) =>
            new Client()
            {
                CodLuna = Convert.ToInt32(rowView.Row["codluna"]),
                Name = Convert.ToString(rowView.Row["clientname"]),
                TotalDebt = Convert.ToSingle(rowView.Row["totaldebt"]),
                DocId = Convert.ToString(rowView.Row["docid"]),
                BaseAddress = Convert.ToString(rowView.Row["baseaddress"]),
                NewAddress = Convert.ToString(rowView.Row["newaddress"]),
                AlternativeAddress = Convert.ToString(rowView.Row["alternativeaddress"]),
                Business = Convert.ToString(rowView.Row["business"]),
                DueRange = Convert.ToInt32(rowView.Row["duerange"]),
                Zonal = Convert.ToString(rowView.Row["zonal"]),
                Sector = Convert.ToString(rowView.Row["sector"]),
                District = Convert.ToString(rowView.Row["district"]),
                ManagementKind = Convert.ToString(rowView.Row["managementkind"])
            };

        public static DisaggregatedDebt GetDebt(DataRowView rowView) =>
            new DisaggregatedDebt()
            {
                Bill = Convert.ToInt32(rowView.Row["bill"]),
                DaysPastDue = Convert.ToInt16(rowView.Row["dayspastdue"]),
                Debt = Convert.ToSingle(rowView.Row["debt"]),
                DueDate = Convert.ToDateTime(rowView.Row["duedate"]),
                PhoneNumber = Convert.ToString(rowView.Row["phonenumber"]),
                Service = Convert.ToString(rowView.Row["service"])
            };
    }
}