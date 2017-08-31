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

            client.CodLuna = Convert.ToInt32(rowView.Row["Código_Cliente"]);
            client.Name = Convert.ToString(rowView.Row["Nombre_Cliente"]);
            client.TotalDebt = Convert.ToSingle(rowView.Row["Deuda_Total"]);
            client.DocId = Convert.ToString(rowView.Row["No_Identificación"]);
            client.BaseAddress = Convert.ToString(rowView.Row["Dirección_Base_Carta"]);
            client.NewAddress1 = Convert.ToString(rowView.Row["Direccion_Nueva1"]);
            client.NewAddress2 = Convert.ToString(rowView.Row["Direccion_Nueva2"]);
            client.AlternativeAddress1 = Convert.ToString(rowView.Row["Direccion_Ubicada1"]);
            client.AlternativeAddress2 = Convert.ToString(rowView.Row["Direccion_Ubicada2"]);
            client.Business = Convert.ToString(rowView.Row["Negocio"]);
            client.DueRange = Convert.ToString(rowView.Row["Rango_Deuda"]);
            client.Zonal = Convert.ToString(rowView.Row["Zonal"]);
            client.Sector = Convert.ToString(rowView.Row["Sector"]);
            client.District = Convert.ToString(rowView.Row["Distrito"]);
            client.ManagementKind = Convert.ToString(rowView.Row["Tipo_Getión"]);

            return client;
        }

        public static DisaggregatedDebt GetDebt(DataRowView rowView)
        {
            var debt = new DisaggregatedDebt();

            debt.Bill = Convert.ToString(rowView.Row["Factura"]);
            debt.DaysPastDue = Convert.ToInt16(rowView.Row["Días_Mora"]);
            debt.Debt = Convert.ToSingle(rowView.Row["Deuda"]);
            debt.DueDate = Convert.ToDateTime(rowView.Row["Fecha_Vencimiento"]);
            debt.PhoneNumber = Convert.ToString(rowView.Row["Número_Teléfono"]);
            debt.Service = Convert.ToString(rowView.Row["Servicio"]);

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