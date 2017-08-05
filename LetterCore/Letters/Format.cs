namespace LetterCore.Letters
{
    using System.Collections.Generic;

    public class Format
    {
        public Format(string url, List<Client> clients, string chargeClazz)
        {
            Url = url;
            Clients = clients;
            ChargeClazz = chargeClazz;
        }

        public string Url { get; }

        public List<Client> Clients { get; }

        public string ChargeClazz { get; }
    }
}
