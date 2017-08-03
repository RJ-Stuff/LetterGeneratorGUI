using LetterCore.Letters;
using Letters;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new InputData();
            Subject<object> progress = new Subject<object>();

            var format1 = new Format($@"{Directory.GetCurrentDirectory()}\formats\tdp-lastchance.rjf", "", WdPaperSize.wdPaperLegal);

            //var configuration72horas = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-72.json");
            //var configuration48horas = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-48.json");
            //var configurationUrgent = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-urgent.json");
            //var configurationLastChance = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-lastchance.json");
            //var configurationNonCompliance = Utils.LoadConfiguration($@"{Directory.GetCurrentDirectory()}\resources\tdp-non-compliance.json");



            AllInOneGenerator.CreateDocs(new List<Format>() { format1 }, progress, WdPaperSize.wdPaperLegal);


            Console.ReadKey();
        }
    }
}
