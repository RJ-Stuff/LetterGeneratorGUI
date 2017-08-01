using Microsoft.Office.Interop.Word;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;

namespace Letters
{

    public class AllInOneGenerator
    {
        public const float PointsToCM = 28.3464567F;

        public static void CreateDocs(InputData input, string output, Subject<object> progress, WdPaperSize paperSize)
        {
            var configuration72horas = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-72.json");
            var configuration48horas = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-48.json");
            var configurationUrgent = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-urgent.json");
            var configurationLastChance = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-lastchance.json");
            var configurationNonCompliance = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-non-compliance.json");

            Application wordApp;
            wordApp = new Application()
            {
                ShowAnimation = false,
                Visible = false
            };
            var document = wordApp.Documents.Add();
            SetPage(document, configuration72horas, paperSize);
            document.Content.Font.Size = 10;
            document.Content.Font.Name = "candara";

            new Letter(configuration72horas, input, new Dictionary<string, int>(), document, progress,
                "72 horas", paperSize == WdPaperSize.wdPaperLegal).CreatePages();
            new Letter(configuration48horas, input, new Dictionary<string, int>(), document, progress,
                "48 horas", paperSize == WdPaperSize.wdPaperLegal).CreatePages();
            new UrgentLetter(configurationUrgent, input, document, progress,
                "Urgente", paperSize == WdPaperSize.wdPaperLegal).CreatePages();
            new Letter(configurationLastChance, input, new Dictionary<string, int>()
            {
                {"SetTextb4Table", 9},
                {"SetTextAfterTable", 9},
                {"SetGeneralPaymentInfo", 9},
                {"SetBusinessURL", 9},
                {"SetFinalInfo", 9}
            }, document, progress, "Última oportunidad de pago", paperSize == WdPaperSize.wdPaperLegal).CreatePages();
            new NonComplianceLetter(configurationNonCompliance, input, new Dictionary<string, int>(), document, progress,
                "Incumplimiento del compromiso de pago", paperSize == WdPaperSize.wdPaperLegal).CreatePages();

            document.Paragraphs.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
            //todo verificar q exista
            document.SaveAs2(Path.Combine(output, "TodoEnUno.docx"));

            progress.OnCompleted();

            document.Close();
            Marshal.ReleaseComObject(document);
            wordApp.Quit();
            Marshal.ReleaseComObject(wordApp);
            wordApp = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void SetPage(Document document, JToken configuration, WdPaperSize paperSize)
        {
            //document.PageSetup.PaperSize = WdPaperSize.wdPaperLegal;
            document.PageSetup.PaperSize = paperSize;
            document.PageSetup.LeftMargin = configuration["LeftMargin"].Value<float>() * PointsToCM;
            document.PageSetup.TopMargin = configuration["TopMargin"].Value<float>() * PointsToCM;
            document.PageSetup.RightMargin = configuration["RightMargin"].Value<float>() * PointsToCM;
            document.PageSetup.BottomMargin = configuration["BottomMargin"].Value<float>() * PointsToCM;
        }
    }
}
