namespace LetterCore.Letters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;

    using Microsoft.Office.Interop.Word;

    using Newtonsoft.Json.Linq;

    class UrgentLetter : Letter
    {
        public UrgentLetter(JToken configuration, List<Client> clients,
            Document document, Subject<object> progress, SimpleCharge charge,
            WdPaperSize paperSize) :
            base(configuration, clients, document, progress, charge, paperSize)
        {
        }

        protected override void SetTextAfterTable(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = this.FontSizes["SetTextAfterTable"];
            paragraph.Range.Font.Name = "Candara";

            var text = this.Configuration["TextAfterTable"].Value<string>();
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
            paragraph.Range.Text = this.Configuration["AfterTableItems"]
                .Values()
                .Select(it => it.Value<string>())
                .Select(s => $"•        {s}\u000B")
                .Aggregate((acc, c) => $"{acc}{c}");
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected override void SetPaymentPlace(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();

            var paymentPlace =
                paragraph.Range.InlineShapes.AddPicture(
                    string.Format(this.Configuration["PaymentPlace"].Value<string>(), this.CurrentDir)
                    );
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
            paragraph.Range.Text = this.Configuration["TextAfterPaymentPlace"].Value<string>();
            paragraph.Range.InsertParagraphAfter();
        }

        protected override void SetBusinessUrl(Document document)
        {
        }

        protected override void SetGeneralPaymentInfo(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = this.FontSizes["SetGeneralPaymentInfo"];
            paragraph.Range.Font.Name = "Candara";

            var text = this.Configuration["GeneralPaymentInfo"].Value<string>();
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
                    string.Format(this.Configuration["LawyerSignature"].Value<string>(), this.CurrentDir)
                    );
            var lawyerSignatureShape = lawyerSignature.ConvertToShape();
            lawyerSignatureShape.Left = Convert.ToSingle(WdShapePosition.wdShapeLeft);
            paragraph.SpaceAfter = 2;

            paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 10;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = $"\v{this.Configuration["LawyerName"].Value<string>()}";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            paragraph.Range.InsertParagraphAfter();
            paragraph.Range.InsertParagraphAfter();
        }

    }
}
