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

    public class LatexController
    {
        private static string id;

        public static string LatexGenerator(
            List<Format> formats,
            string paperSize,
            string chargeClazz,
            BackgroundWorker worker,
            DoWorkEventArgs e,
            int limit)
        {
            CheckTempDirectory();

            id = $"{DateTime.Now:dd-MM-yyyy-hh-mm-ss}";

            var bodyFormat = File.ReadAllText(Combine(Directory.GetCurrentDirectory(), @"latex\letters.tex"));

            var filename = $@"temp\letters-{id}";

            var texBody = bodyFormat
                .Replace("%INCLUDEFILES%", string.Join(string.Empty, formats.Select(f => CreateFormat(f, worker, e, limit))))
                .Replace("%INCLUDECHARGES%", CreateCharges(formats.SelectMany(f => f.Clients), chargeClazz));

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

            if (latexProcess.ExitCode != 0) throw new Exception("Ha ocurrido un error en la creación del PDF.");

            return result;
        }

        private static string CreateCharges(IEnumerable<Client> clients, string chargeClazz)
        {
            return string.Join(
                "\r\n",
                clients
                .Select(c => GetCharge(chargeClazz, c))
                .Select(f => GetFileNameWithoutExtension(f))
                .Select(f => $@"\include{{{f}}}"));
        }

        private static string GetCharge(string chargeClazz, Client c)
        {
            var chargeFormat = File.ReadAllText(Combine(Directory.GetCurrentDirectory(), "charges", chargeClazz));

            var newAddresses = new List<string>()
            { c.NewAddress1, c.NewAddress2, c.AlternativeAddress1, c.AlternativeAddress2 }
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

            var chargeBody = chargeFormat
                .Replace("%DNI%", c.DocId)
                .Replace("%CODLUNA%", Convert.ToString(c.CodLuna))
                .Replace("%NAME%", c.Name)
                .Replace("%TOTALDEBT%", Convert.ToString(c.TotalDebt))
                .Replace("%BASEADD%", c.BaseAddress)
                .Replace("%NEWADD%", string.Join("\\\\", newAddresses));

            var filename = $@"temp\{chargeClazz}-{id}-{c.CodLuna}";

            File.WriteAllText($"{filename}.tex", chargeBody);

            return filename;
        }

        private static void CheckTempDirectory()
        {
            if (Directory.Exists("temp")) return;

            Directory.CreateDirectory("temp");
        }

        private static string CreateFormat(Format f, BackgroundWorker worker, DoWorkEventArgs e, int limit)
        {
            var progressCount = 0;

            var includeFiles = f.Clients
                .Select(c => ProcessReactiveClient(f, worker, e, c, ref progressCount, limit))
                .Select(s => $@"\include{{{s}}}");

            return string.Join("\r\n", includeFiles);
        }

        private static string ProcessReactiveClient(
            Format f, 
            BackgroundWorker worker, 
            DoWorkEventArgs e, 
            Client c, 
            ref int progressCount,
            int limit)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return string.Empty;
            }

            var result = GetFileName(ProcessClient(f, c, limit));
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

        private static string ProcessClient(Format f, Client c, int limit)
        {
            var bodyFormat = File.ReadAllText(f.Url);

            var texBody = bodyFormat
                .Replace("%NAME%", c.Name)
                .Replace("%ADDRESS%", c.BaseAddress)
                .Replace("%TOTALDEBT%", Convert.ToString(c.TotalDebt))
                .Replace("%DEBTS%", GetDebtsTable(c.DisaggregatedDebts, limit));

            var filename = $@"temp\{GetFileNameWithoutExtension(f.Url)}-{id}-{c.CodLuna}";

            File.WriteAllText($"{filename}.tex", texBody);

            return filename;
        }

        private static string GetDebtsTable(IEnumerable<DisaggregatedDebt> debts, int limit)
        {
            var table = string.Join(
                @"\hline ",
                debts
                .Take(limit)
                .Select(d => $"{d.Bill} & {d.Service} & {d.PhoneNumber} & {d.DueDate:d} & {d.DaysPastDue} & {d.Debt:C2}\\\\")
                .ToArray());

            return debts.Count() > limit ? $"{table} \\hline - & Otros. & - & - & - & -\\\\" : table;
        }
    }
}
