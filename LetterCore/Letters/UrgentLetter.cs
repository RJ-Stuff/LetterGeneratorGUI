namespace LetterCore.Letters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Subjects;

    using Microsoft.Office.Interop.Word;

    using Newtonsoft.Json.Linq;

    class UrgentLetter : Letter
    {
        public UrgentLetter(
            JToken configuration,
            List<Client> clients,
            Document document,
            Charge charge,
            WdPaperSize paperSize, BackgroundWorker worker, DoWorkEventArgs e) :
            base(configuration, clients, document, charge, paperSize, worker, e)
        {
        }

        protected override void SetTextAfterTable(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = FontSizes["SetTextAfterTable"];
            paragraph.Range.Font.Name = "Candara";

            var text = Configuration["TextAfterTable"].Value<string>().Replace("\\v", "\v");
            var start = paragraph.Range.Start + text.IndexOf("$");
            var end = paragraph.Range.Start + text.LastIndexOf("$");

            paragraph.Range.Text = text.Replace("$", string.Empty);

            var rng = document.Range(start, end);
            rng.Font.Underline = WdUnderline.wdUnderlineSingle;

            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();

            paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 9;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Font.Bold = 1;
            paragraph.Range.Text = Configuration["AfterTableItems"]
                .Values()
                .Select(it => it.Value<string>())
                .Select(s => $"•        {s}\v")
                .Aggregate((acc, c) => $"{acc}{c}");
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected override void SetPaymentPlace(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();

            var paymentPlace =
                paragraph.Range.InlineShapes.AddPicture(
                    string.Format(Configuration["PaymentPlace"].Value<string>(), CurrentDir));
            var paymentPlaceShape = paymentPlace.ConvertToShape();
            paymentPlaceShape.Left = Convert.ToSingle(WdShapePosition.wdShapeCenter);
            paragraph.Range.InsertParagraphAfter();
            paragraph.Range.InsertParagraphAfter();
            paragraph.Range.InsertParagraphAfter();
            paragraph.Range.InsertParagraphAfter();
            paragraph.Range.InsertParagraphAfter();

            paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 9;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = Configuration["TextAfterPaymentPlace"].Value<string>().Replace("\\v", "\v");
            paragraph.Range.InsertParagraphAfter();
        }

        protected override void SetBusinessUrl(Document document)
        {
        }

        protected override void SetGeneralPaymentInfo(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = FontSizes["SetGeneralPaymentInfo"];
            paragraph.Range.Font.Name = "Candara";

            var text = Configuration["GeneralPaymentInfo"].Value<string>().Replace("\\v", "\v");
            var start = paragraph.Range.Start + text.IndexOf("$");
            var end = paragraph.Range.Start + text.LastIndexOf("$");

            var start2 = paragraph.Range.Start + text.IndexOf("%") - 2;
            var end2 = paragraph.Range.Start + text.LastIndexOf("%") - 3;

            paragraph.Range.Text = text.Replace("$", string.Empty).Replace("%", string.Empty);

            var rng = document.Range(start, end);
            rng.Bold = 1;

            var rng2 = document.Range(start2, end2);
            rng2.Font.Bold = 1;
            rng2.Font.Underline = WdUnderline.wdUnderlineSingle;
            rng2.Font.Color = WdColor.wdColorBlue;

            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
            paragraph.SpaceAfter = 0;
        }

        protected override void SetSignature(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            var lawyerSignature =
                paragraph.Range.InlineShapes.AddPicture(
                    string.Format(Configuration["LawyerSignature"].Value<string>(), CurrentDir));
            var lawyerSignatureShape = lawyerSignature.ConvertToShape();
            lawyerSignatureShape.Left = Convert.ToSingle(WdShapePosition.wdShapeLeft);
            paragraph.SpaceAfter = 2;

            paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 10;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = $"\v{Configuration["LawyerName"].Value<string>().Replace("\\v", "\v")}";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            paragraph.Range.InsertParagraphAfter();
            paragraph.Range.InsertParagraphAfter();
        }

    }
}
