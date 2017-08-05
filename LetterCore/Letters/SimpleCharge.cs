namespace LetterCore.Letters
{
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Office.Interop.Word;

    public class SimpleCharge
    {
        public virtual void CreateDocument(Document document, Client client, string businessName)
        {
            var parCount = document.Paragraphs.Count;
            var offset = document.Paragraphs[parCount - 1].Range.End;

            var fullPath = Assembly.GetAssembly(typeof(SimpleCharge)).Location;
            var currentDir = Path.GetDirectoryName(fullPath);

            var path = $@"{currentDir}\charges\simplecharge.txt";
            
            var text = File.ReadAllText(path);
            text = string.Format(
                text,
                businessName,
                client.DocId,
                client.CodLuna,
                client.Name,
                client.TotalDebt,
                client.BaseAddress,
                client.NewAddress,
                client.AlternativeAddress);

            var paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Text = text.Replace("$", string.Empty);
            paragraph.Range.Font.Size = 8;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            var position = 0;
            var count = 1;

            text
                .Select((c, i) => c == '$' ? new { start = i + 1, end = 0, pos = position++ } : null)
                .Where(pair => pair != null && pair.pos % 2 == 0)
                .Select(p => new { p.start, end = text.IndexOf('$', p.start), count = Inc(ref count) })
                .Select(p => new { start = p.start - p.count, end = p.end - p.count })
                .Select(p => document.Range(p.start + offset, p.end + offset))
                .ToList()
                .ForEach(r => r.Bold = 1);

            paragraph.Range.InsertParagraphAfter();
        }

        private static int Inc(ref int count)
        {
            var oldValue = count;
            count += 2;
            return oldValue;
        }
    }
}
