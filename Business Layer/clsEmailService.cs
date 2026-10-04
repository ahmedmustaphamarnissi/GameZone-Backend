using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer;

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

public class clsEmailService
{
    private readonly IConfiguration _config;

    public clsEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> SendEmailAsync(
        string recipientEmail,
        string subject,
        string htmlBody)
    {
        try
        {
            var host = _config["EmailSettings:Host"];
            var port = int.Parse(_config["EmailSettings:Port"]!);
            var userName = _config["EmailSettings:UserName"];
            var password = Environment.GetEnvironmentVariable("EmailServicePassword");
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderName = _config["EmailSettings:SenderName"];

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(senderName, senderEmail)
            );

            message.To.Add(
                MailboxAddress.Parse(recipientEmail)
            );

            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = "Thank you for using GameZone. Please view this email in an HTML-compatible client."
            };

            message.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync(
                host,
                port,
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                userName,
                password
            );

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email sending failed: {ex.Message}");
            return false;
        }
    }
}
