namespace LetterApp.model
{
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    using Newtonsoft.Json;

    class Format
    {
        public string Url { get; set; }

        public bool Checked { get; set; }

        public PaperSize PaperSize { get; set; }

        public Charge Charge { get; set; }

        public List<Filter> Filters { get; set; }

        [JsonIgnore]
        public BindingSource BindingSource { get; set; }

        public Format()
        {
        }

        public Format(string url) : this(url, true, new List<Filter>(), PaperSize.DefaultSize, Charge.DefaultCharge)
        {
        }

        private Format(string url, bool Checked, List<Filter> Filters, PaperSize paperSize, Charge charge)
        {
            Url = url;
            this.Checked = Checked;
            PaperSize = paperSize;
            Charge = charge;
            this.Filters = Filters;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + Url.GetHashCode();
                hash = hash * 23 + Checked.GetHashCode();
                hash = hash * 23 + PaperSize.GetHashCode();

                return hash;
            }
        }

        public override string ToString()
        {
            return Path.GetFileNameWithoutExtension(Url);
        }
    }
}
