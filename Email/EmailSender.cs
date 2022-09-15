using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using demoasm2.sendEmail;

namespace demoasm2.sendEmail
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<EmailSenderOptions> options)
        {
            this.Options = options.Value;
        }

        public EmailSenderOptions Options { get; set; }

        //public Task SendEmailAsync(string email, string subject, string htmlmessage)
        //{
        //    return Execute(email, subject, htmlmessage);
        //}
        //public Task SendEmailAsync(string email, string subject, string htmlMessage);

        public async Task Execute(string to, string subject, string htmlmessage)
        {
            // create message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(Options.Sender);
            if (!string.IsNullOrEmpty(Options.Name))
                email.Sender.Name = Options.Name;
            email.From.Add(email.Sender);
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlmessage };

            // send email
            //using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            //{
            //    smtp.Connect(Options.Host, Options.Port, Options.Host_SecureSocketOptions);
            //    smtp.Authenticate(Options.User, Options.Pass);
            //    smtp.Send(email);
            //    smtp.Disconnect(true);
            //}
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect(Options.Host, Options.Port, Options.Host_SecureSocketOptions);
                smtp.Authenticate(Options.User, Options.Pass);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
               
            }

            smtp.Disconnect(true);

           

           Task.FromResult(true);
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }
    }
}
