using System.Collections.Generic;

namespace LetterCore.latex
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using LetterCore.Letters;

    using static System.IO.Path;

    class LatexController
    {
        private static string id;

        public static string LatexGenerator(
            List<Format> formats,
            string paperSize,
            string chargeClazz,
            BackgroundWorker worker,
            DoWorkEventArgs e)
        {
            CheckTempDirectory();

            id = $"{DateTime.Now:dd-MM-yyyy-hh-mm-ss}";

            var bodyFormat = File.ReadAllText(Combine(Directory.GetCurrentDirectory(), @"latex\letters.tex"));
            var filename = $@"temp\letters-{id}";
            var texBody = bodyFormat
                .Replace("%INCLUDEFILES%", string.Join(string.Empty, formats.Select(f => CreateFormat(f, chargeClazz, worker, e))))
                .Replace("%PAPERSIZE%", paperSize)
                .Replace("%TOPMARGIN%", paperSize == "a4paper" ? "2.8cm" : "-1cm");

            File.WriteAllText($"{filename}.tex", texBody);
            var result = $"cartas-{id}.pdf";

            var latexProcess = new Process
            {
                StartInfo =
                    {
                        FileName = "compile.bat",
                        Arguments = $"{filename} cartas-{id}.pdf",
                        UseShellExecute = false
                    }
            };

            latexProcess.Start();
            latexProcess.WaitForExit();

            if (latexProcess.ExitCode != 0) throw new Exception("An error occurred.");

            return result;
        }

        private static void CheckTempDirectory()
        {
            if (Directory.Exists("temp")) return;

            Directory.CreateDirectory("temp");
        }

        private static string CreateFormat(Format f, string chargeClazz, BackgroundWorker worker, DoWorkEventArgs e)
        {
            var progressCount = 0;

            var includeFiles = f.Clients
                .Select(c => ProcessReactiveClient(f, chargeClazz, worker, e, c, ref progressCount))
                .Select(s => $@"\include{{{s}}}");

            return string.Join("\r\n", includeFiles);
        }

        private static string ProcessReactiveClient(Format f, string chargeClazz, BackgroundWorker worker, DoWorkEventArgs e, Client c, ref int progressCount)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return string.Empty;
            }

            var result = GetFileName(ProcessClient(f, chargeClazz, c));
            var letterKind = GetFileNameWithoutExtension(f.Url);
            var count = f.Clients.Count;

            worker.ReportProgress(0, new ProgressIncrement()
            {
                Count = count,
                Progress = ++progressCount,
                Information = $"Cartas del tipo {letterKind}: {progressCount} de {count}"
            });

            return result;
        }

        private static string ProcessClient(Format f, string chargeClazz, Client c)
        {
            var bodyFormat = File.ReadAllText(f.Url);

            var chargeFormat = !string.IsNullOrEmpty(chargeClazz)
                                   ? File.ReadAllText(
                                       Path.Combine(Directory.GetCurrentDirectory(), "charges", chargeClazz))
                                   : string.Empty;

            var chargeBody = chargeFormat
                .Replace("%DNI%", c.DocId)
                .Replace("%CODLUNA%", Convert.ToString(c.CodLuna))
                .Replace("%NAME%", c.Name)
                .Replace("%TOTALDEBT%", Convert.ToString(c.TotalDebt))
                .Replace("%BASEADD%", c.BaseAddress)
                .Replace("%NEWADD%", c.NewAddress);

            var texBody = bodyFormat
                .Replace("%NAME%", c.Name)
                .Replace("%ADDRESS%", c.BaseAddress)
                .Replace("%TOTALDEBT%", Convert.ToString(c.TotalDebt))
                .Replace("%DEBTS%", GetDebtsTable(c.DisaggregatedDebts))
                .Replace("%CHARGE%", chargeBody);

            var filename = $@"temp\{GetFileNameWithoutExtension(f.Url)}-{id}";

            File.WriteAllText($"{filename}.tex", texBody);

            return filename;
        }

        private static string GetDebtsTable(IEnumerable<DisaggregatedDebt> debts)
        {
            return string.Join(
                @"\hline ",
                debts
                .Select(d => $"{d.Bill} & {d.Service} & {d.PhoneNumber} & {d.DueDate:d} & {d.DaysPastDue} & {d.Debt}\\\\")
                .ToArray());
        }
    }
}
