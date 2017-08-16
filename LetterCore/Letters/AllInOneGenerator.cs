namespace LetterCore.Letters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    using LetterCore.latex;

    using Microsoft.Office.Interop.Word;

    using Newtonsoft.Json.Linq;

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
