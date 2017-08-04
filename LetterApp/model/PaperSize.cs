using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;

namespace LetterApp.model
{
    class PaperSize
    {
        public static readonly PaperSize DEFAULT_SIZE =
            new PaperSize("wdPaperA4", "Papel A4",
                new List<Charge>() { Charge.DEFAULT_CHARGE });

        public WdPaperSize Papersize { get; set; }
        public string DisplayName { get; set; }
        public List<Charge> Charges { get; set; }

        public PaperSize() { }

        public PaperSize(string paperSize, string displayName, List<Charge> Charges)
        {
            Papersize = (WdPaperSize)Enum.Parse(typeof(WdPaperSize), paperSize);
            DisplayName = displayName;
            this.Charges = Charges;
        }

        public PaperSize(int paperSize, string displayName, List<Charge> Charges)
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
            this.Charges = Charges;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + Papersize.ToString().GetHashCode();
                hash = hash * 23 + DisplayName.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
