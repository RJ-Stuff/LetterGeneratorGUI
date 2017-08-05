namespace LetterCore.Letters
{
    using System;
    using System.Collections.Generic;

    public class DisaggregatedDebt
    {
        public int Bill { get; }
        public string Service { get; }
        public string PhoneNumber { get; }
        public DateTime DueDate { get; }
        public short DaysPastDue { get; }
        public float Debt { get; }
    }

    public class Client
    {
        public int CodLuna { get; set; }
        public string Name { get; set; }
        public float TotalDebt { get; set; }
        public string DocId { get; set; }
        public string BaseAddress { get; set; }
        public string NewAddress { get; set; }
        public string AlternativeAddress { get; set; }
        public string Business { get; set; }
        public int DueRange { get; set; }
        public string Zonal { get; set; }
        public string Sector { get; set; }
        public string District { get; set; }
        public string ManagementKind { get; set; }
        public List<DisaggregatedDebt> DisaggregatedDebts { get; set; }
    }
}
