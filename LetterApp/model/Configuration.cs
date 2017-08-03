using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LetterApp.model
{
    class Configuration
    {
        public List<Format> Formats;
        public List<string> Notifications;

        public Configuration()
        {
            Formats = new List<Format>();
            Notifications = new List<string>();
        }

        public void Add(string URL)
        {
            Add(new Format(URL));
        }

        public void Add(Format format)
        {
            Formats.Add(format);
            Persist();
        }

        public void SetFormats(List<Format> Formats)
        {
            this.Formats = Formats;
            Persist();
        }

        public void SetNotifications(List<string> Notifications)
        {
            this.Notifications = Notifications;
            Persist();
        }

        private void Persist()
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "configuration.json"), jsonString, Encoding.Default);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un problema al guardar la configuración.", "Error");
            }
        }
    }
}
