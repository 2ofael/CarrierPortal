using System.Collections.Generic;
using System.IO;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarrierPortal.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        EmailSettings _emailSettings = new EmailSettings();

        public EmailService()
        {


            _smtpServer = "sandbox.smtp.mailtrap.io";
            _smtpPort = 465;
            _smtpUsername = "c9e62558b590f4";
            _smtpPassword = "6b32bdf1eb3b19";

            _emailSettings.SmtpServer = _smtpServer;
            _emailSettings.SmtpPort = _smtpPort;
            _emailSettings.SmtpUsername = _smtpUsername;
            _emailSettings.SmtpPassword = _smtpPassword;
            _emailSettings.SenderName = _smtpUsername;
            _emailSettings.SenderEmail = "Hello";


        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body, string attachmentFilePath = null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress("hello", recipientEmail));
            message.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;

            if (!string.IsNullOrEmpty(attachmentFilePath))
            {
                var attachment = new MimePart()
                {
                    Content = new MimeContent(System.IO.File.OpenRead(attachmentFilePath), ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = System.IO.Path.GetFileName(attachmentFilePath)
                };

                builder.Attachments.Add(attachment);
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
