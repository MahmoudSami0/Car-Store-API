using CarStore.Applcation.Services;
using CarStore.InfraStructure.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CarStore.InfraStructure.Services;

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;

    public MailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailSettings.FromName,_mailSettings.FromAddress));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Confirm Your Email";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .button {{
                            background-color: #4CAF50;
                            border: none;
                            color: white;
                            padding: 15px 32px;
                            text-align: center;
                            text-decoration: none;
                            display: inline-block;
                            font-size: 16px;
                            margin: 20px 0;
                            cursor: pointer;
                            border-radius: 5px;
                        }}
                    </style>
                </head>
                <body>
                    <h2>Confirm Your Email</h2>
                    <p>Thank you for registering. Please confirm your email address by  clicking     the     button  below:</p>
                    <a href='{confirmationLink}' class='button'>Confirm Email</a>
                    <p>If you didn't request this, please ignore this email.</p>
                </body>
                </html>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _mailSettings.SmtpServer, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_mailSettings.SmtpUsername, _mailSettings.SmtpPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}