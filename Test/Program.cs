namespace Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reactive.Subjects;

    using LetterCore.Letters;

    using Microsoft.Office.Interop.Word;

    class Program
    {
        static void Main(string[] args)
        {
            ////var configuration72horas = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-72.json");
            ////var configuration48horas = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-48.json");
            ////var configurationUrgent = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-urgent.json");
            ////var configurationLastChance = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-lastchance.json");
            ////var configurationNonCompliance = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-non-compliance.json");
            var progress = new Subject<object>();
            var input = new InputData();

            var format1 = new Format(
                $@"{Directory.GetCurrentDirectory()}\formats\tdp-72.rjf",
                input.GetClients(),
                null);

            AllInOneGenerator.CreateDocs(new List<Format> { format1 }, progress, WdPaperSize.wdPaperLegal);
            
            // Console.ReadKey();
        }
    }
}
