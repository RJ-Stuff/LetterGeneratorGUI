using System.Collections.Generic;
using System.IO;

namespace LetterApp.model
{
    class Format
    {
        public string URL { get; set; }
        public bool Checked { get; set; }
        public List<Filter> Filters { get; set; }
        public PaperSize PaperSize { get; set; }
        public Charge Charge { get; set; }

        public Format() { }

        public Format(string URL) : this(URL, true, new List<Filter>(), PaperSize.DEFAULT_SIZE, Charge.DEFAULT_CHARGE)
        {

        }

        public Format(string URL, bool Checked, List<Filter> Filters, PaperSize PaperSize, Charge Charge)
        {
            this.URL = URL;
            this.Checked = Checked;
            this.Filters = Filters;
            this.PaperSize = PaperSize;
            this.Charge = Charge;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + URL.GetHashCode();
                hash = hash * 23 + Checked.GetHashCode();
                hash = hash * 23 + Filters.GetHashCode();
                hash = hash * 23 + PaperSize.GetHashCode();

                return hash;
            }
        }

        public override string ToString()
        {
            return Path.GetFileNameWithoutExtension(URL);
        }
    }
}
