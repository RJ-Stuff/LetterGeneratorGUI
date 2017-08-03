using Letters;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
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
            AllInOneGenerator.CreateDocs(input, @"C:\Users\rigo\Desktop\output\TodoEnUno.docx", progress, WdPaperSize.wdPaperLegal);
        }
    }
}
