namespace LetterApp.model
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Office.Interop.Word;

    class PaperSize
    {
        public static readonly PaperSize DefaultSize =
            new PaperSize("wdPaperA4", "Papel A4",
                new List<Charge> { Charge.DefaultCharge });

        public WdPaperSize Papersize { get; set; }
        public string DisplayName { get; set; }
        public List<Charge> Charges { get; set; }

        public PaperSize() { }

        public PaperSize(string paperSize, string displayName, List<Charge> charges)
        {
            Papersize = (WdPaperSize)Enum.Parse(typeof(WdPaperSize), paperSize);
            DisplayName = displayName;
            this.Charges = charges;
        }

        public PaperSize(int paperSize, string displayName, List<Charge> charges)
        {
            if (Enum.IsDefined(typeof(WdPaperSize), paperSize))
            {
                Papersize = (WdPaperSize)paperSize;
            }
            else
            {
                Papersize = WdPaperSize.wdPaperLegal;
            }

            DisplayName = displayName;
            this.Charges = charges;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + Papersize.ToString().GetHashCode();
                hash = hash * 23 + DisplayName.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetHashCode() == GetHashCode();
        }
    }
}
