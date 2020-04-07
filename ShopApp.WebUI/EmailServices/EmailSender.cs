using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.EmailServices
{
    public class EmailSender : IEmailSender
    {
        // SendGrid API Key - Github push etmeden silinmesi gerekmektedir.
        private const string SendGridKey = "";

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(SendGridKey, subject, message, email);
        }

        private Task Execute(string sendGridKey, string subject, string message, string email)
        {
            var client = new SendGridClient(sendGridKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("info@shopapp.com", "ShopApp"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            msg.AddTo(new EmailAddress(email));

            return client.SendEmailAsync(msg);
        }
    }
}