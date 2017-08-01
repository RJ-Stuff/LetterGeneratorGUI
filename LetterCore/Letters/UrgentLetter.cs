using Microsoft.Office.Interop.Word;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Letters
{
    class UrgentLetter : Letter
    {
        public UrgentLetter(JToken configuration, InputData input, Document document,
            Subject<object> progress, string letterKind, bool useCharge) :
            base(configuration, input, new Dictionary<string, int>() { { "SetTextb4Table", 9 } },
                document, progress, letterKind, useCharge)
        {
        }

        protected override void SetTextAfterTable(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = FontSizes["SetTextAfterTable"];
            paragraph.Range.Font.Name = "Candara";

            var text = configuration["TextAfterTable"].Value<string>();
            var start = paragraph.Range.Start + text.IndexOf("$");
            var end = paragraph.Range.Start + text.LastIndexOf("$");

            paragraph.Range.Text = text.Replace("$", "");

            var rng = document.Range(start, end);
            rng.Font.Underline = WdUnderline.wdUnderlineSingle;

            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();

            paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 9;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Font.Bold = 1;
            paragraph.Range.Text = configuration["AfterTableItems"]
                .Values()
                .Select(it => it.Value<string>())
                .Select(s => $"•        {s}\u000B")
                .Aggregate((acc, c) => $"{acc}{c}");
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
        }

        protected override void SetPaymentPlace(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();

            var PaymentPlace =
                paragraph.Range.InlineShapes.AddPicture(configuration["PaymentPlace"].Value<string>());
            var PaymentPlaceShape = PaymentPlace.ConvertToShape();
            PaymentPlaceShape.Left = Convert.ToSingle(WdShapePosition.wdShapeCenter);
            paragraph.Range.InsertParagraphAfter();
            paragraph.Range.InsertParagraphAfter();
            paragraph.Range.InsertParagraphAfter();
            paragraph.Range.InsertParagraphAfter();
            paragraph.Range.InsertParagraphAfter();

            paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 9;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = configuration["TextAfterPaymentPlace"].Value<string>();
            paragraph.Range.InsertParagraphAfter();
        }

        protected override void SetBusinessURL(Document document)
        {
        }

        protected override void SetGeneralPaymentInfo(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = FontSizes["SetGeneralPaymentInfo"];
            paragraph.Range.Font.Name = "Candara";

            var text = configuration["GeneralPaymentInfo"].Value<string>();
            var start = paragraph.Range.Start + text.IndexOf("$");
            var end = paragraph.Range.Start + text.LastIndexOf("$");

            var start2 = paragraph.Range.Start + text.IndexOf("%") - 2;
            var end2 = paragraph.Range.Start + text.LastIndexOf("%") - 3;

            paragraph.Range.Text = text.Replace("$", "").Replace("%", "");

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
            Paragraph paragraph = document.Content.Paragraphs.Add();
            var LawyerSignature =
                paragraph.Range.InlineShapes.AddPicture(configuration["LawyerSignature"].Value<string>());
            var LawyerSignatureShape = LawyerSignature.ConvertToShape();
            LawyerSignatureShape.Left = Convert.ToSingle(WdShapePosition.wdShapeLeft);
            paragraph.SpaceAfter = 2;

            paragraph = document.Content.Paragraphs.Add();
            paragraph.Range.Font.Size = 10;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = $"{"\u000B"}{configuration["LawyerName"].Value<string>()}";
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            paragraph.Range.InsertParagraphAfter();
            paragraph.Range.InsertParagraphAfter();
        }

    }
}
