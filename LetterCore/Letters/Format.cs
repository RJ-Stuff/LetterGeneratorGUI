using Letters;
using System.Collections.Generic;

namespace LetterCore.Letters
{
    public class Format
    {
        public string URL { get; }
        public List<Client> Clients { get; }
        public string ChargeClazz { get; }

        public Format(string URL, List<Client> Clients, string ChargeClazz)
        {
            this.URL = URL;
            this.Clients = Clients;
            this.ChargeClazz = ChargeClazz;
        }
    }
}
