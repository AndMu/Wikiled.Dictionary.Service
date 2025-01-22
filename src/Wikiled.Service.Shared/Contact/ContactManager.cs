using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using NLog;
using Wikiled.Sercice.Shared.Contact;

namespace Wikiled.Service.Shared.Contact
{
    public class ContactManager : IContactManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public async Task Send(ContactForm form)
        {
            logger.Debug("Send");
            try
            {
                using (var client = new SmtpClient())
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Wikiled Info", "info@wikiled.com"));
                    message.To.Add(new MailboxAddress("Andrius", "keistokas@gmail.com"));
                    message.Subject = $"Service message [{form.App}-{form.From}] - {form.Subject}";
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("<p>Andriau,</p>");
                    builder.AppendLine($"<p>Service message from {form.App} {form.From}</p>");
                    builder.AppendLine($"<p><b>{form.Subject}</b></p>");
                    builder.AppendLine($"<p>{form.Message}</p>");

                    message.Body = new TextPart("html") { Text = builder.ToString() };
                    await client.ConnectAsync("smtp.livemail.co.uk", 465, true)
                                .ConfigureAwait(false);
                    
                    await client.AuthenticateAsync("info@wikiled.com", "Kla1peda").ConfigureAwait(false);
                    await client.SendAsync(message, CancellationToken.None).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
