using Application.DTOs.SMTPConfig;
using Application.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Serilog;

namespace Infrastructure.Helpers;

public class EmailService(IOptions<SmtpConfigurationModel> smtpConfig) : IEmailService
{
    public async Task<string> SendEmailAsync(string to, string subject, string text)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtpConfig.Value.Username, smtpConfig.Value.Email));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = $"{text}"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpConfig.Value.Host, smtpConfig.Value.Port);
            await smtp.AuthenticateAsync(smtpConfig.Value.Email, smtpConfig.Value.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            return "Sent email";
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to send email");
            return "Something went wrong";
        }
    }
}