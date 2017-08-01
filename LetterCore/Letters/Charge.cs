using Microsoft.Office.Interop.Word;
using System.IO;
using System.Linq;

namespace Letters
{
    public class Charge
    {
        public static void CreateDocument(Document document, Client client, string BusinessName, string template)
        {
            var parCount = document.Paragraphs.Count;
            var offset = document.Paragraphs[parCount - 1].Range.End;

            var text = File.ReadAllText(template);
            text = string.Format(text, BusinessName, client.DocID, client.CodLuna, client.Name, client.TotalDebt,
                client.BaseAddress, client.NewAddress, client.AlternativeAddress);

            Paragraph paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Text = text.Replace("$", "");
            paragraph.Range.Font.Size = 8;
            paragraph.Range.Font.Name = "candara";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            int position = 0;
            int count = 1;

            text
                .Select((c, i) => c == '$' ? new { start = i + 1, end = 0, pos = position++ } : null)
                .Where(pair => pair != null && pair.pos % 2 == 0)
                .Select(p => new { start = p.start, end = text.IndexOf('$', p.start), count = Inc(ref count) })
                .Select(p => new { start = p.start - p.count, end = p.end - p.count })
                .Select(p => document.Range(p.start + offset, p.end + offset))
                .ToList()
                .ForEach(r => r.Bold = 1);

            paragraph.Range.InsertParagraphAfter();
        }

        private static int Inc(ref int count)
        {
            int oldValue = count;
            count += 2;
            return oldValue;
        }
    }
}
