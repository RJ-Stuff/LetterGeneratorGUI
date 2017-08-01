using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LetterApp.model
{
    class Format
    {
        public string URL { get; set; }
        public bool Checked { get; set; }
        public List<string> Filters { get; set; }
        public List<string> Notification { get; set; }

        public Format(string URL) : this(URL, true, new List<string>(), new List<string>())
        {

        }

        public Format(string URL, bool Checked, List<string> Filters, List<string> Notification)
        {
            this.URL = URL;
            this.Checked = Checked;
            this.Filters = Filters;
            this.Notification = Notification;
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

        public Configuration()
        {
            Formats = new List<Format>();
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
