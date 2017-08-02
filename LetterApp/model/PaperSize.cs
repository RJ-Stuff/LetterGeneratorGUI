using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetterApp.model
{
    class PaperSize
    {
        public static readonly PaperSize DEFAULT_SIZE = new PaperSize("wdPaperLegal", "Papel Oficio");

        public WdPaperSize Papersize { get; set; }
        public string DisplayName { get; set; }

        public PaperSize() { }

        public PaperSize(string paperSize, string displayName)
        {
            Papersize = (WdPaperSize)Enum.Parse(typeof(WdPaperSize), paperSize);
            DisplayName = displayName;
        }

        public PaperSize(int paperSize, string displayName)
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
