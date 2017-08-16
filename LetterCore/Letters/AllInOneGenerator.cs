namespace LetterCore.Letters
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using LetterCore.latex;

    using Microsoft.Office.Interop.Word;

    public class AllInOneGenerator
    {
        public static string CreateDocs(List<Format> formats, WdPaperSize paperSize, BackgroundWorker worker, DoWorkEventArgs e)
        {
            return LatexController.LatexGenerator(
                formats,
                paperSize == WdPaperSize.wdPaperLegal ? "legalpaper" : "a4paper",
                worker,
                e);
        }
    }
}
