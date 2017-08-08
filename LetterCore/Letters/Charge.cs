namespace LetterCore.Letters
{
    using Microsoft.Office.Interop.Word;

    public abstract class Charge
    {
        public abstract void CreateDocument(Document document, Client client, string businessName);
    }
}