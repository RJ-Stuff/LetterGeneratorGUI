using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LetterApp.model
{
    class Format
    {
        public string URL { get; set; }
        public bool Checked { get; set; }
        public List<Filter> Filters { get; set; }
        public PaperSize PaperSize { get; set; }

        public Format() { }

        public Format(string URL) : this(URL, true, new List<Filter>(), PaperSize.DEFAULT_SIZE)
        {

        }

        public Format(string URL, bool Checked, List<Filter> Filters, PaperSize PaperSize)
        {
            this.URL = URL;
            this.Checked = Checked;
            this.Filters = Filters;
            this.PaperSize = PaperSize;
        }

        public override int GetHashCode()
        {
            return Path.GetFileNameWithoutExtension(URL).GetHashCode();
        }

        public override string ToString()
        {
            return Path.GetFileNameWithoutExtension(URL);
        }
    }

    class Configuration
    {
        public List<Format> Formats { get; set; }
        public List<string> Notification { get; set; }

        public Configuration()
        {
            Formats = new List<Format>();
            Notification = new List<string>();
        }

        public void Add(string URL)
        {
            var hasFormat = Formats.Any(f => f.URL == URL);
            if (!hasFormat) Formats.Add(new Format(URL));
        }

        public void Add(Format format)
        {
            var hasFormat = Formats.Any(f => f.URL == format.URL);
            if (!hasFormat) Formats.Add(format);
        }
    }
}
