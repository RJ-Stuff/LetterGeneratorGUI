using LetterCore.Letters;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Letters
{

    public class AllInOneGenerator
    {
        public static void CreateDocs(List<Format> formats, Subject<object> progress, WdPaperSize paperSize)
        {
            Application wordApp;
            wordApp = new Application()
            {
                ShowAnimation = false,
                Visible = false
            };

            var document = wordApp.Documents.Add();
            document.Content.Font.Size = 10;
            document.Content.Font.Name = "Candara";

            formats.ForEach(format =>
            {
                var configuration = Utils.LoadConfiguration(format.URL);
                var clazz = configuration["classname"].Value<string>();
                var clients = format.Clients;

                var assembly = Assembly.GetExecutingAssembly();

                var chargeType = format.ChargeClazz != null && format.ChargeClazz.Length != 0
                ? assembly.GetTypes().First(t => t.Name == format.ChargeClazz)
                : null;

                var charge = (chargeType != null ? Activator.CreateInstance(chargeType) : null) as SimpleCharge;

                var type = assembly.GetTypes().First(t => t.Name == clazz);
                var parameters = new Object[] { configuration, clients, document, progress, charge, paperSize };
                var letter = Activator.CreateInstance(type, parameters);
                var method = type.GetMethod("CreatePages");

                method.Invoke(letter, null);
            });

            document.Paragraphs.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            document.SaveAs2(Path.Combine(Directory.GetCurrentDirectory(), "TodoEnUno.docx"));

            progress.OnCompleted();

            document.Close();
            Marshal.ReleaseComObject(document);
            wordApp.Quit();
            Marshal.ReleaseComObject(wordApp);
            wordApp = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
