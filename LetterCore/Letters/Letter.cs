using Microsoft.Office.Interop.Word;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using static Letters.AllInOneGenerator;

namespace Letters
{
    public class Letter
    {
        public const float PointsToCM = 28.3464567F;

        protected JToken configuration;
        protected List<Client> clients;
        protected string businessName;
        protected string letterKind;
        protected Dictionary<string, int> FontSizes;
        protected Document document;
        protected Subject<object> progress;
        protected int progressCount;
        protected SimpleCharge charge;
        protected string currentDir;

        public Letter(JToken configuration, List<Client> clients,
            Document document, Subject<object> progress, SimpleCharge charge,
            WdPaperSize paperSize)
        {
            this.document = document;
            this.clients = clients;
            this.configuration = configuration;
            this.progress = progress;
            this.charge = charge;
            letterKind = configuration["letterKind"].Value<string>();
            currentDir = Directory.GetCurrentDirectory();

            SetPage(this.document, configuration, paperSize);

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
            document.PageSetup.LeftMargin = configuration["LeftMargin"].Value<float>() * PointsToCM;
            document.PageSetup.TopMargin = configuration["TopMargin"].Value<float>() * PointsToCM;
            document.PageSetup.RightMargin = configuration["RightMargin"].Value<float>() * PointsToCM;
            document.PageSetup.BottomMargin = configuration["BottomMargin"].Value<float>() * PointsToCM;
        }

        protected virtual void UpdateProgress()
        {
            progress.OnNext(new { count = clients.Count, progress = ++progressCount, information = $"Creando cartas del tipo: {letterKind}" });
        }

        public virtual void CreatePages()
        {
            this.progressCount = 0;
            clients.ForEach(c =>
            {
                CreatePage(document, c);
                document.Words.Last.InsertBreak(WdBreakType.wdPageBreak);
                UpdateProgress();
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
            SetBusinessURL(document);
            SetPaymentPlace(document);
            SetSignature(document);
            SetFinalInfo(document);
            if (charge != null) SetPseudoFooter(document, client);
        }

        protected virtual void SetPseudoFooter(Document document, Client client)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();

            var line = paragraph.Range.InlineShapes.AddHorizontalLineStandard();
            line.Height = 1.5F;
            line.Fill.Solid();
            line.HorizontalLineFormat.NoShade = true;
            line.Fill.ForeColor.RGB = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
            line.HorizontalLineFormat.Alignment = WdHorizontalLineAlignment.wdHorizontalLineAlignCenter;
            paragraph.SpaceAfter = 0;

            paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Text = configuration["FooterInfo"].Value<string>();
            paragraph.Range.Font.Size = 8;
            paragraph.Range.Font.Name = "candara";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            object charUnit = WdUnits.wdCharacter;
            object move = -5F * PointsToCM;
            var rng = paragraph.Range;
            rng.MoveStart(charUnit, move);

            paragraph.Range.InsertParagraphAfter();

            line = paragraph.Range.InlineShapes.AddHorizontalLineStandard();
            line.Height = 1F;
            line.Fill.Solid();
            line.HorizontalLineFormat.NoShade = true;
            line.Fill.ForeColor.RGB = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
            line.HorizontalLineFormat.Alignment = WdHorizontalLineAlignment.wdHorizontalLineAlignCenter;
            paragraph.SpaceAfter = 0;

            charge.CreateDocument(document, client, businessName);
        }

        protected virtual void SetFinalInfo(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes.ContainsKey("SetFinalInfo") ? FontSizes["SetTextb4Table"] : 9;
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = configuration["FinalInfo"].Value<string>()
                .Replace("$$$", DateTime.Now.ToString("dd/MM/yyyy"));
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetSignature(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 10;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = "Atentamente,\u000B";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();

            paragraph = document.Content.Paragraphs.Add();
            var LawyerSignature =
                paragraph.Range.InlineShapes.AddPicture(
                    String.Format(configuration["LawyerSignature"].Value<string>(), currentDir)
                    );
            var LawyerSignatureShape = LawyerSignature.ConvertToShape();
            LawyerSignatureShape.Left = Convert.ToSingle(WdShapePosition.wdShapeLeft);
            paragraph.SpaceAfter = 0;

            paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 10;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = $"{"\u000B"}{configuration["LawyerName"].Value<string>()}";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            object charUnit = WdUnits.wdCharacter;
            object move = -4F * PointsToCM;
            var rng = paragraph.Range;
            rng.MoveStart(charUnit, move);

            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetPaymentPlace(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();

            var PaymentPlace =
                paragraph.Range.InlineShapes.AddPicture(
                    string.Format(configuration["PaymentPlace"].Value<string>(), currentDir)
                    );
        }

        protected virtual void SetBusinessURL(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes.ContainsKey("SetBusinessURL") ? FontSizes["SetTextb4Table"] : 10;
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Font.Bold = 1;
            paragraph.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
            paragraph.Range.Font.Color = WdColor.wdColorBlue;
            paragraph.Range.Text = configuration["BusinessURL"].Value<string>();
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetGeneralPaymentInfo(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes.ContainsKey("SetGeneralPaymentInfo") ? FontSizes["SetTextb4Table"] : 10;
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";

            var text = configuration["GeneralPaymentInfo"].Value<string>();
            var start = paragraph.Range.Start + text.IndexOf("$");
            var end = paragraph.Range.Start + text.LastIndexOf("$");

            paragraph.Range.Text = text.Replace("$", "");

            var rng = document.Range(start, end);
            rng.Bold = 1;

            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetTextAfterTable(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes.ContainsKey("SetTextAfterTable") ? FontSizes["SetTextb4Table"] : 10;
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = configuration["TextAfterTable"].Value<string>();
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetTextb4Table(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes.ContainsKey("SetTextb4Table") ? FontSizes["SetTextb4Table"] : 10;
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = configuration["Textb4Table"].Value<string>();
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetTotalDebt(Document document, float totalDebt)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();

            Table table = document.Tables.Add(paragraph.Range, 1, 2);
            table.Range.Font.Size = 12;
            table.Range.Font.Name = "Candara";
            table.Range.Font.Bold = 1;

            table.Borders.Enable = 1;

            table.Cell(1, 1).Range.Text = "Deuda Total";
            table.Cell(1, 2).Range.Text = $"S/. {totalDebt}";
            table.Rows.SetHeight(0.56F * PointsToCM, WdRowHeightRule.wdRowHeightExactly);// = 0.28F * PointsToCM;

            table.Columns.Width = 3.5F * PointsToCM;
            table.Rows.Alignment = WdRowAlignment.wdAlignRowRight;

            table.Cell(1, 1).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetClientDebts(Document document, Client client)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();

            Table table = document.Tables.Add(paragraph.Range, client.DisaggregatedDebts.Count + 1, 6);
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
            table.Rows[1].SetHeight(0.44F * PointsToCM, WdRowHeightRule.wdRowHeightExactly);

            table.Columns[1].Width = 1.51F * PointsToCM;
            table.Columns[2].Width = 2.48F * PointsToCM;
            table.Columns[3].Width = 4.26F * PointsToCM;
            table.Columns[4].Width = 3.24F * PointsToCM;
            table.Columns[5].Width = 2.5F * PointsToCM;
            table.Columns[6].Width = 2F * PointsToCM;

            var r = 2;
            client.DisaggregatedDebts.ForEach(d =>
            {
                table.Cell(r, 1).Range.Text = d.Bill.ToString();
                table.Cell(r, 2).Range.Text = d.Service;
                table.Cell(r, 3).Range.Text = d.PhoneNumber;
                table.Cell(r, 4).Range.Text = d.DueDate.ToString("dd-MM-yyyy");
                table.Cell(r, 5).Range.Text = d.DaysPastDue.ToString();
                table.Cell(r, 6).Range.Text = $"S/. {d.Debt}";

                Enumerable
                .Range(1, 6)
                .ToList()
                .ForEach(i => table.Cell(r, i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter);

                table.Rows[r].SetHeight(0.48F * PointsToCM, WdRowHeightRule.wdRowHeightExactly);
                r++;
            });

            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetClient(Document document, Client client)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 10;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = $"Señor(a):\u000B{client.Name}\u000B{client.BaseAddress}";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetTitle(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 17;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Font.Bold = 1;
            paragraph.Range.Text = configuration["Title"].Value<string>();
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            paragraph.Range.InsertParagraphAfter();
        }

        protected virtual void SetPseudoHeader(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();

            if (charge != null)
            {
                var BusinessLogo =
                paragraph.Range.InlineShapes.AddPicture(
                    string.Format(configuration["BusinessLogo"].Value<string>(), currentDir)
                    );
                var Logo =
                    paragraph.Range.InlineShapes.AddPicture(
                        string.Format(configuration["Logo"].Value<string>(), currentDir)
                        );
                var LogoShape = Logo.ConvertToShape();
                LogoShape.Left = Convert.ToSingle(WdShapePosition.wdShapeRight);
                LogoShape.Top = Convert.ToSingle(WdShapePosition.wdShapeTop);
            }
            else
            {
                paragraph.Range.InsertParagraphAfter();
            }
            paragraph.SpaceAfter = 0;
        }

        protected virtual void SetDate(Document document, DateTime date)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 10;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = String.Format("{0}, {1} de {2} de {3}",
                configuration["CurrentCity"], date.Day, date.ToString("MMMM").ToLower(), date.Year);
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;

            object charUnit = WdUnits.wdCharacter;
            object move = -5F * PointsToCM;
            var rng = paragraph.Range;
            rng.MoveStart(charUnit, move);

            paragraph.Range.InsertParagraphAfter();
        }
    }
}
