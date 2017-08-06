namespace LetterApp.model
{
    using System.IO;

    using Newtonsoft.Json;

    class Format
    {
        public string Url { get; set; }

        public bool Checked { get; set; }

        public PaperSize PaperSize { get; set; }

        public Charge Charge { get; set; }

        [JsonIgnore]
        public object DataSource { get; set; }

        public Format()
        {

        }

        public Format(string url) : this(url, true, PaperSize.DefaultSize, Charge.DefaultCharge)
        {
        }

        private Format(string url, bool Checked, PaperSize paperSize, Charge charge)
        {
            this.Url = url;
            this.Checked = Checked;
            this.PaperSize = paperSize;
            this.Charge = charge;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + this.Url.GetHashCode();
                hash = hash * 23 + Checked.GetHashCode();
                hash = hash * 23 + PaperSize.GetHashCode();

                return hash;
            }
        }

        public override string ToString()
        {
            return Path.GetFileNameWithoutExtension(this.Url);
        }
    }
}
