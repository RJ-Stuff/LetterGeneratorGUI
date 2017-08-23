namespace LetterApp.view
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Windows.Forms;

    using LetterCore.Letters;

    public class ViewUtils
    {
        public static IEnumerable<List<DataRowView>> GroupBy(IEnumerable<DataRowView> data, string key) =>
            data.GroupBy(row => row[key], (k, group) => group.ToList());
        //todo actualizar en consulta el nombre de las columnas.
        public static Client GetClient(DataRowView rowView) =>
            new Client()
            {
                CodLuna = Convert.ToInt32(rowView.Row["codluna"]),
                Name = Convert.ToString(rowView.Row["clientname"]),
                TotalDebt = Convert.ToSingle(rowView.Row["totaldebt"]),
                DocId = Convert.ToString(rowView.Row["docid"]),
                BaseAddress = Convert.ToString(rowView.Row["baseaddress"]),
                NewAddress = Convert.ToString(rowView.Row["newaddress"]),
                AlternativeAddress = Convert.ToString(rowView.Row["alternativeaddress"]),
                Business = Convert.ToString(rowView.Row["business"]),
                DueRange = Convert.ToInt32(rowView.Row["duerange"]),
                Zonal = Convert.ToString(rowView.Row["zonal"]),
                Sector = Convert.ToString(rowView.Row["sector"]),
                District = Convert.ToString(rowView.Row["district"]),
                ManagementKind = Convert.ToString(rowView.Row["managementkind"])
            };

        public static DisaggregatedDebt GetDebt(DataRowView rowView) =>
            new DisaggregatedDebt()
            {
                Bill = Convert.ToInt32(rowView.Row["bill"]),
                DaysPastDue = Convert.ToInt16(rowView.Row["dayspastdue"]),
                Debt = Convert.ToSingle(rowView.Row["debt"]),
                DueDate = Convert.ToDateTime(rowView.Row["duedate"]),
                PhoneNumber = Convert.ToString(rowView.Row["phonenumber"]),
                Service = Convert.ToString(rowView.Row["service"])
            };

        public static void SendNotification(List<string> emails, string username, string password, string attachment)
        {
            try
            {
                var msg = new MailMessage();

                emails.ForEach(e => msg.To.Add(e));
                msg.From = new MailAddress(username, "RJAbogados - Reporte Automático");
                msg.Subject = $"Notificación de la generación de cartas - {DateTime.Now:d}";
                msg.Body = "Un saludo cordial.";
                msg.IsBodyHtml = true;
                if (!string.IsNullOrEmpty(attachment))
                {
                    msg.Attachments.Add(new Attachment(Path.Combine(Directory.GetCurrentDirectory(), attachment)));
                }

                var client = new SmtpClient
                {
                    Host = "smtp.office365.com",
                    Credentials = new System.Net.NetworkCredential(username, password),
                    Port = 587,
                    EnableSsl = true
                };

                client.Send(msg);

                MessageBox.Show("Notificación enviada correctamente", "información");
            }
            catch (Exception e)
            {
                var msg = $"Ocurrió un problema al enviar la notificación\n\nDescripción:\n{e.Message}";
                MessageBox.Show(msg, "Error");
            }
        }
    }
}