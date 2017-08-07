namespace LetterApp.model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Newtonsoft.Json;

    class Configuration
    {
        public List<Format> Formats;
        public List<string> Notifications;

        public Configuration()
        {
            Formats = new List<Format>();
            Notifications = new List<string>();
        }

        public void AddFormat(string url)
        {
            AddFormat(new Format(url));
        }

        public void AddFormat(Format format)
        {
            Formats.Add(format);
            Persist();
        }

        public void SetFormats(List<Format> formats)
        {
            Formats = formats;
            Persist();
        }

        public void SetNotifications(List<string> notifications)
        {
            Notifications = notifications;
            Persist();
        }

        private void Persist()
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "configuration.json"), jsonString, Encoding.Default);
            }
            catch (Exception)
            {
                MessageBox.Show("Ocurrió un problema al guardar la configuración.", "Error");
            }
        }
    }
}
