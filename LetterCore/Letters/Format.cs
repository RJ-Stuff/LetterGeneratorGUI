using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetterCore.Letters
{
    class Format
    {
        public string URL { get; set; }
        public string Filters { get; set; }
        public WdPaperSize PaperSize { get; set; }

        public Format(string URL, string Filters, WdPaperSize PaperSize)
        {
            this.URL = URL;
            this.Filters = Filters;
            this.PaperSize = PaperSize;
        }
    }
}
