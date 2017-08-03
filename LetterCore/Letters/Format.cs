using Letters;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetterCore.Letters
{
    public class Format
    {
        public string URL { get; set; }
        public List<Client> Clients { get; set; }

        public Format(string URL, List<Client> Clients)
        {
            this.URL = URL;
            this.Clients = Clients;
        }
    }
}
