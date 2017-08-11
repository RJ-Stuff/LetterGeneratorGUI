namespace LetterCore.Letters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    using Microsoft.Office.Interop.Word;

    using Newtonsoft.Json.Linq;

    public class AllInOneGenerator
    {
        public static string CreateDocs(List<Format> formats, WdPaperSize paperSize, BackgroundWorker worker, DoWorkEventArgs e)
        {
            var wordApp = new Application
            {
                ShowAnimation = false,
                Visible = false
            };

            var document = wordApp.Documents.Add();
            document.Content.Font.Size = 10;
            document.Content.Font.Name = "Candara";

            formats.ForEach(format =>
            {
                if (e.Cancel) return;
                var configuration = Utils.LoadConfiguration(format.Url);
                var clazz = configuration["classname"].Value<string>();
                var clients = format.Clients;

                var assembly = Assembly.GetExecutingAssembly();

                var chargeType = !string.IsNullOrEmpty(format.ChargeClazz)
                                     ? assembly.GetTypes().First(t => t.Name == format.ChargeClazz)
                                     : null;

                var charge = (chargeType != null ? Activator.CreateInstance(chargeType) : null) as Charge;

                var type = assembly.GetTypes().First(t => t.Name == clazz);
                var parameters = new object[] { configuration, clients, document, charge, paperSize, worker, e };
                var letter = Activator.CreateInstance(type, parameters);
                var method = type.GetMethod("CreatePages");

                method.Invoke(letter, null);
            });

            var docName = string.Empty;
            if (!e.Cancel)
            {
                docName = $"TodoEnUno-{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.docx";

                document.Paragraphs.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                document.SaveAs2(Path.Combine(Directory.GetCurrentDirectory(), docName));
            }

            document.Close(e.Cancel ? WdSaveOptions.wdDoNotSaveChanges : WdSaveOptions.wdSaveChanges);
            Marshal.ReleaseComObject(document);
            document = null;
            wordApp.Quit();
            Marshal.ReleaseComObject(wordApp);
            wordApp = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            return docName;
        }
    }
}
