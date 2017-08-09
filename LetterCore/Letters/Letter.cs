namespace LetterCore.Letters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reactive.Subjects;

    using Microsoft.Office.Interop.Word;

    using Newtonsoft.Json.Linq;

    public class Letter
    {
        public const float PointsToCm = 28.3464567F;

        protected JToken Configuration;
        protected List<Client> Clients;
        protected string BusinessName;
        protected string LetterKind;
        protected Dictionary<string, int> FontSizes;
        protected Document Document;
        protected int ProgressCount;
        protected Charge Charge;
        protected string CurrentDir;
        protected WdPaperSize paperSize;
        protected BackgroundWorker worker;
        protected DoWorkEventArgs bgEvent;

        public Letter(JToken configuration, List<Client> clients,
            Document document, Charge charge,
            WdPaperSize paperSize, BackgroundWorker worker, DoWorkEventArgs e)
        {
            this.worker = worker;
            this.bgEvent = e;
            Document = document;
            Clients = clients;
            Configuration = configuration;
            Charge = charge;
            LetterKind = configuration["letterKind"].Value<string>();
            BusinessName = configuration["BusinessName"].Value<string>();
            CurrentDir = Directory.GetCurrentDirectory();

            this.paperSize = paperSize;

            SetPage(Document, configuration, paperSize);

            FontSizes = new Dictionary<string, int>
            {
                ["SetTextb4Table"] = 10,
                ["SetTextAfterTable"] = 10,
                ["SetGeneralPaymentInfo"] = 10,
                ["SetBusinessURL"] = 10,
                ["SetFinalInfo"] = 9
            };

            configuration["fontsizes"]
                .ToList()
                .ForEach(f =>
                {
                    var key = f["part"].Value<string>();
                    var value = f["size"].Value<int>();
                    FontSizes[key] = value;
                });
        }

        private void SetPage(Document document, JToken configuration, WdPaperSize paperSize)
        {
            document.PageSetup.PaperSize = WdPaperSize.wdPaperLegal;
            document.PageSetup.PaperSize = paperSize;
            document.PageSetup.LeftMargin = configuration["LeftMargin"].Value<float>() * PointsToCm;
            document.PageSetup.TopMargin = configuration["TopMargin"].Value<float>() * PointsToCm;
            document.PageSetup.RightMargin = configuration["RightMargin"].Value<float>() * PointsToCm;
            document.PageSetup.BottomMargin = configuration["BottomMargin"].Value<float>() * PointsToCm;
        }
        
        public virtual void CreatePages()
        {
            ProgressCount = 0;
            Clients.ForEach(c =>
            {
                if (worker.CancellationPending)
                {
                    bgEvent.Cancel = true;
                }
                else
                {
                    CreatePage(Document, c);
                    Document.Words.Last.InsertBreak(WdBreakType.wdPageBreak);
                    worker.ReportProgress(0, new ProgressIncrement()
                    {
                        Count = Clients.Count,
                        Progress = ++ProgressCount,
                        Information = $"Cartas del tipo {LetterKind}: {ProgressCount} de {Clients.Count}"
                    });
                }
            });
        }

        protected virtual void CreatePage(Document document, Client client)
        {
            SetPseudoHeader(document);
            SetDate(document, DateTime.Now);
            SetTitle(document);
            SetClient(document, client);
            SetTotalDebt(document, client.TotalDebt);
            SetTextb4Table(document);
            SetClientDebts(document, client);
            SetTextAfterTable(document);
            SetGeneralPaymentInfo(document);
            SetBusinessUrl(document);
            SetPaymentPlace(document);
            SetSignature(document);
            SetFinalInfo(document);
            if (paperSize == WdPaperSize.wdPaperLegal)
                SetPseudoFooter(document, client);
            if (paperSize == WdPaperSize.wdPaperA4)
                Charge?.CreateDocument(document, client, BusinessName);
        }

        protected virtual void SetPseudoFooter(Document document, Client client)
        {
            var paragraph = document.Content.Paragraphs.Add();

            var line = paragraph.Range.InlineShapes.AddHorizontalLineStandard();
            line.Height = 1.5F;
            line.Fill.Solid();
            line.HorizontalLineFormat.NoShade = true;
            line.Fill.ForeColor.RGB = ColorTranslator.ToOle(Color.Black);
            line.HorizontalLineFormat.Alignment = WdHorizontalLineAlignment.wdHorizontalLineAlignCenter;
            paragraph.SpaceAfter = 0;

            paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Text = Configuration["FooterInfo"].Value<string>().Replace("\\v", "\v");
            paragraph.Range.Font.Size = 8;
            paragraph.Range.Font.Name = "candara";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            object charUnit = WdUnits.wdCharacter;
            object move = -5F * PointsToCm;
            var rng = paragraph.Range;
            rng.MoveStart(charUnit, move);

            paragraph.Range.InsertParagraphAfter();

            line = paragraph.Range.InlineShapes.AddHorizontalLineStandard();
            line.Height = 1F;
            line.Fill.Solid();
            line.HorizontalLineFormat.NoShade = true;
            line.Fill.ForeColor.RGB = ColorTranslator.ToOle(Color.Black);
            line.HorizontalLineFormat.Alignment = WdHorizontalLineAlignment.wdHorizontalLineAlignCenter;
            paragraph.SpaceAfter = 0;

            Charge.CreateDocument(document, client, BusinessName);
        }

        protected virtual void SetFinalInfo(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes.ContainsKey("SetFinalInfo") ? FontSizes["SetTextb4Table"] : 9;
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = Configuration["FinalInfo"].Value<string>()
                .Replace("\\v", "\v")
                .Replace("$$$", DateTime.Now.ToString("dd/MM/yyyy"));
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetSignature(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 10;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = "Atentamente,\v";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();

            paragraph = document.Content.Paragraphs.Add();
            var lawyerSignature =
                paragraph.Range.InlineShapes.AddPicture(
                    string.Format(Configuration["LawyerSignature"].Value<string>(), CurrentDir));
            var lawyerSignatureShape = lawyerSignature.ConvertToShape();
            lawyerSignatureShape.Left = Convert.ToSingle(WdShapePosition.wdShapeLeft);
            paragraph.SpaceAfter = 0;

            paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 10;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = $"\v{Configuration["LawyerName"].Value<string>().Replace("\\v", "\v")}";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            object charUnit = WdUnits.wdCharacter;
            object move = -4F * PointsToCm;
            var rng = paragraph.Range;
            rng.MoveStart(charUnit, move);

            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetPaymentPlace(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();

            paragraph.Range.InlineShapes.AddPicture(
                string.Format(Configuration["PaymentPlace"].Value<string>(), CurrentDir));
        }

        protected virtual void SetBusinessUrl(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes.ContainsKey("SetBusinessURL") ? FontSizes["SetTextb4Table"] : 10;
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Font.Bold = 1;
            paragraph.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
            paragraph.Range.Font.Color = WdColor.wdColorBlue;
            paragraph.Range.Text = Configuration["BusinessURL"].Value<string>().Replace("\\v", "\v");
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetGeneralPaymentInfo(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes.ContainsKey("SetGeneralPaymentInfo") ? FontSizes["SetTextb4Table"] : 10;
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";

            var text = Configuration["GeneralPaymentInfo"].Value<string>().Replace("\\v", "\v");
            var start = paragraph.Range.Start + text.IndexOf("$");
            var end = paragraph.Range.Start + text.LastIndexOf("$");

            paragraph.Range.Text = text.Replace("$", string.Empty);

            var rng = document.Range(start, end);
            rng.Bold = 1;

            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetTextAfterTable(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes.ContainsKey("SetTextAfterTable") ? FontSizes["SetTextb4Table"] : 10;
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = Configuration["TextAfterTable"].Value<string>().Replace("\\v", "\v");
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetTextb4Table(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes.ContainsKey("SetTextb4Table") ? FontSizes["SetTextb4Table"] : 10;
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = Configuration["Textb4Table"].Value<string>().Replace("\\v", "\v");
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetTotalDebt(Document document, float totalDebt)
        {
            var paragraph = document.Content.Paragraphs.Add();

            var table = document.Tables.Add(paragraph.Range, 1, 2);
            table.Range.Font.Size = 12;
            table.Range.Font.Name = "Candara";
            table.Range.Font.Bold = 1;

            table.Borders.Enable = 1;

            table.Cell(1, 1).Range.Text = "Deuda Total";
            table.Cell(1, 2).Range.Text = $"S/. {totalDebt:0.00}";
            table.Rows.SetHeight(0.56F * PointsToCm, WdRowHeightRule.wdRowHeightExactly);

            table.Columns.Width = 3.5F * PointsToCm;
            table.Rows.Alignment = WdRowAlignment.wdAlignRowRight;

            table.Cell(1, 1).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetClientDebts(Document document, Client client)
        {
            var paragraph = document.Content.Paragraphs.Add();

            var table = document.Tables.Add(paragraph.Range, client.DisaggregatedDebts.Count + 1, 6);
            table.Borders.Enable = 1;
            table.Cell(1, 1).Range.Text = "Factura";
            table.Cell(1, 2).Range.Text = "Servicio";
            table.Cell(1, 3).Range.Text = "Teléfono/Código Cliente";
            table.Cell(1, 4).Range.Text = "F. Vencimiento";
            table.Cell(1, 5).Range.Text = "Días de Mora";
            table.Cell(1, 6).Range.Text = "Deuda";
            Enumerable
                .Range(1, 6)
                .ToList()
                .ForEach(i => table.Cell(1, i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter);

            table.Rows[1].Range.Font.Size = 10;
            table.Rows[1].Range.Font.Name = "Candara";
            table.Rows[1].Range.Font.Bold = 1;
            table.Rows[1].SetHeight(0.44F * PointsToCm, WdRowHeightRule.wdRowHeightExactly);

            table.Columns[1].Width = 1.51F * PointsToCm;
            table.Columns[2].Width = 2.48F * PointsToCm;
            table.Columns[3].Width = 4.26F * PointsToCm;
            table.Columns[4].Width = 3.24F * PointsToCm;
            table.Columns[5].Width = 2.5F * PointsToCm;
            table.Columns[6].Width = 2F * PointsToCm;

            var r = 2;
            client.DisaggregatedDebts.ForEach(d =>
            {
                table.Cell(r, 1).Range.Text = d.Bill.ToString();
                table.Cell(r, 2).Range.Text = d.Service;
                table.Cell(r, 3).Range.Text = d.PhoneNumber;
                table.Cell(r, 4).Range.Text = d.DueDate.ToString("dd-MM-yyyy");
                table.Cell(r, 5).Range.Text = d.DaysPastDue.ToString();
                table.Cell(r, 6).Range.Text = $"S/. {d.Debt:0.00}";

                Enumerable
                .Range(1, 6)
                .ToList()
                .ForEach(i => table.Cell(r, i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter);

                table.Rows[r].SetHeight(0.48F * PointsToCm, WdRowHeightRule.wdRowHeightExactly);
                r++;
            });

            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetClient(Document document, Client client)
        {
            var paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 10;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = $"Señor(a):\v{client.Name}\v{client.BaseAddress}";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetTitle(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 17;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Font.Bold = 1;
            paragraph.Range.Text = Configuration["Title"].Value<string>().Replace("\\v", "\v");
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetPseudoHeader(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();

            if (paperSize == WdPaperSize.wdPaperLegal)
            {
                paragraph.Range.InlineShapes.AddPicture(
                    string.Format(Configuration["BusinessLogo"].Value<string>(), CurrentDir));

                var logo = paragraph.Range.InlineShapes.AddPicture(
                    string.Format(Configuration["Logo"].Value<string>(), CurrentDir));

                var logoShape = logo.ConvertToShape();
                logoShape.Left = Convert.ToSingle(WdShapePosition.wdShapeRight);
                logoShape.Top = Convert.ToSingle(WdShapePosition.wdShapeTop);
            }
            else
            {
                paragraph.Range.InsertParagraphAfter();
            }

            paragraph.SpaceAfter = 0;
        }

        protected virtual void SetDate(Document document, DateTime date)
        {
            var paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 10;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text =
                $"{Configuration["CurrentCity"]}, {date.Day} de {date.ToString("MMMM").ToLower()} de {date.Year}";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;

            object charUnit = WdUnits.wdCharacter;
            object move = -5F * PointsToCm;
            var rng = paragraph.Range;
            rng.MoveStart(charUnit, move);

            paragraph.Range.InsertParagraphAfter();
        }
    }
}
