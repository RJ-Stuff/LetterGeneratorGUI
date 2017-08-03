using Microsoft.Office.Interop.Word;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Letters
{
    public class NonComplianceLetter : Letter
    {
        public NonComplianceLetter(JToken configuration, InputData input, 
            Dictionary<string, int> FontSizes, Document document, Subject<object> progress, 
            string letterKind, bool useCharge) :
            base(configuration, input, FontSizes, document, progress, letterKind, useCharge)
        {
        }

        protected override void SetTextb4Table(Document document)
        {
            Paragraph paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes["SetTextb4Table"];
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = configuration["Textb4Table"].Value<string>()
                .Replace("$$$", DateTime.Now.ToString("dd/MM/yyyy"));
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
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

            var start2 = paragraph.Range.Start + text.IndexOf("%");
            var end2 = paragraph.Range.Start + text.LastIndexOf("%") - 1;

            paragraph.Range.Text = text.Replace("%", "");

            var rng2 = document.Range(start2, end2);
            rng2.Font.Bold = 1;
            rng2.Font.Underline = WdUnderline.wdUnderlineSingle;
            rng2.Font.Color = WdColor.wdColorBlue;

            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            paragraph.Range.InsertParagraphAfter();
            paragraph.SpaceAfter = 0;
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
        }
    }
}
