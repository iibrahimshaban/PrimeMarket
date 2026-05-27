using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using MailSettings = SurveyBasket.Settings.MailSettings;

namespace PrimeMarket.Services.Authentication
{
    public class EmailService(IOptions<MailSettings> options) : IEmailSender
    {
        private readonly MailSettings _mailSettings = options.Value;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(_mailSettings.Password);

            var msg = new SendGridMessage
            {
                From = new EmailAddress(_mailSettings.Mail, _mailSettings.DisplayName),
                Subject = subject,
                HtmlContent = htmlMessage
            };

            msg.AddTo(new EmailAddress(email));

            await client.SendEmailAsync(msg);
        }
    }
}