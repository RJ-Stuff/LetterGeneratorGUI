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

        public static Client GetClient(DataRowView rowView)
        {
            var client = new Client();

            client.CodLuna = Convert.ToInt32(rowView.Row["codluna"]);
            client.Name = Convert.ToString(rowView.Row["clientname"]);
            client.TotalDebt = Convert.ToSingle(rowView.Row["totaldebt"]);
            client.DocId = Convert.ToString(rowView.Row["docid"]);
            client.BaseAddress = Convert.ToString(rowView.Row["baseaddress"]);
            client.NewAddress = Convert.ToString(rowView.Row["newaddress"]);
            client.AlternativeAddress = Convert.ToString(rowView.Row["alternativeaddress"]);
            client.Business = Convert.ToString(rowView.Row["business"]);
            client.DueRange = Convert.ToString(rowView.Row["duerange"]);
            client.Zonal = Convert.ToString(rowView.Row["zonal"]);
            client.Sector = Convert.ToString(rowView.Row["sector"]);
            client.District = Convert.ToString(rowView.Row["district"]);
            client.ManagementKind = Convert.ToString(rowView.Row["managementkind"]);

            return client;
        }

        public static DisaggregatedDebt GetDebt(DataRowView rowView)
        {
            var debt = new DisaggregatedDebt();

            debt.Bill = Convert.ToString(rowView.Row["bill"]);
            debt.DaysPastDue = Convert.ToInt16(rowView.Row["dayspastdue"]);
            debt.Debt = Convert.ToSingle(rowView.Row["debt"]);
            debt.DueDate = Convert.ToDateTime(rowView.Row["duedate"]);
            debt.PhoneNumber = Convert.ToString(rowView.Row["phonenumber"]);
            debt.Service = Convert.ToString(rowView.Row["service"]);

            return debt;
        }

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