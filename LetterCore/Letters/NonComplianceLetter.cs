namespace LetterCore.Letters
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Subjects;

    using Microsoft.Office.Interop.Word;

    using Newtonsoft.Json.Linq;

    public class NonComplianceLetter : Letter
    {
        public NonComplianceLetter(JToken configuration, List<Client> clients,
            Document document, Subject<object> progress, SimpleCharge charge,
            WdPaperSize paperSize) :
            base(configuration, clients, document, progress, charge, paperSize)
        {
        }

        protected override void SetTextb4Table(Document document)
        {
            var paragraph = document.Content.Paragraphs.Add();
            var size = FontSizes["SetTextb4Table"];
            paragraph.Range.Font.Size = size;
            paragraph.Range.Font.Name = "Candara";
            paragraph.Range.Text = Configuration["Textb4Table"].Value<string>()
                .Replace("\\v", "\v")
                .Replace("$$$", DateTime.Now.ToString("dd/MM/yyyy"));
            paragraph.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
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

            var start2 = paragraph.Range.Start + text.IndexOf("%");
            var end2 = paragraph.Range.Start + text.LastIndexOf("%") - 1;

            paragraph.Range.Text = text.Replace("%", string.Empty);

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
        }
    }
}
