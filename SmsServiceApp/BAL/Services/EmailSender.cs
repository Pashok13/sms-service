using BAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Model.DTOs;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace WebApp.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        public const string from = "smssssender@gmail.com";
        public const string code = "quicksender123";

        public EmailSender(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public Task SendEmailAsync(string email, string subject, string message)
		{
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(from, code)
            };
            var mail = new MailMessage(from, email);
			mail.Subject = subject;
			mail.Body = message;
			mail.IsBodyHtml = true;
			return client.SendMailAsync(mail);
		}

        public Task SendEmail(EmailDTO emailDTO)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587) 
            { 
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(from, code)
            };
            MailMessage email = new MailMessage()
            {
                From = new MailAddress(emailDTO.SenderEmail),
                Body = emailDTO.MessageText,
                IsBodyHtml = true,
                Sender = new MailAddress(emailDTO.SenderEmail)
            };
            email.To.Add(new MailAddress(emailDTO.RecepientEmail));
            return client.SendMailAsync(email);
        }

        public void SendEmails(IEnumerable<EmailDTO> emails)
        {
            foreach (EmailDTO email in emails)
            {
                SendEmail(email);
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    scope.ServiceProvider.GetService<IEmailMailingManager>().MarkAs(email, 1);
                }
            }
        }
    }
}
