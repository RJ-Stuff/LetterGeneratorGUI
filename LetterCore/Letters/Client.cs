using System;
using System.Collections.Generic;

namespace Letters
{
    public class DisaggregatedDebt
    {
        public int Bill { get; }
        public string Service { get; }
        public string PhoneNumber { get; }
        public DateTime DueDate { get; }
        public short DaysPastDue { get; }
        public float Debt { get; }

        public DisaggregatedDebt(int Bill, string Service, string PhoneNumber, DateTime DueDate, short DaysPastDue, float Debt)
        {
            this.Bill = Bill;
            this.Service = Service;
            this.PhoneNumber = PhoneNumber;
            this.DueDate = DueDate;
            this.DaysPastDue = DaysPastDue;
            this.Debt = Debt;
        }
    }

    public class Client
    {
        public int CodLuna { get; }
        public string Name { get; }
        public float TotalDebt { get; }
        public string DocID { get; set; }
        public string BaseAddress { get; set; }
        public string NewAddress { get; set; }
        public string AlternativeAddress { get; set; }
        public List<DisaggregatedDebt> DisaggregatedDebts { get; set; }

        public Client(int CodLuna, string Name, float TotalDebt, 
            string DocID, string BaseAddress, string NewAddress, string AlternativeAddress,
            List<DisaggregatedDebt> DisaggregatedDebts)
        {
            this.CodLuna = CodLuna;
            this.Name = Name;
            this.TotalDebt = TotalDebt;
            this.DocID = DocID;
            this.BaseAddress = BaseAddress;
            this.NewAddress = NewAddress;
            this.AlternativeAddress = AlternativeAddress;
            this.DisaggregatedDebts = DisaggregatedDebts;
        }
    }
}
