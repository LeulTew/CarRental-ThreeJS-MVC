using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Carrental.Services;
using Microsoft.Extensions.Options;

public class EmailSender : IEmailSender
{
    private readonly AuthMessageSenderOptions _options;

    public EmailSender(IOptions<AuthMessageSenderOptions> options)
    {
        _options = options.Value;
    }

    public Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            var client = BuildSmtpClient();
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_options.Email ?? "no-reply@example.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to send email. See inner exception for details.", ex);
        }
    }

    public async Task SendForgotPasswordEmailAsync(string email, string callbackUrl)
    {
        try
        {
            var client = BuildSmtpClient();
            var resetPasswordMessage = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>";
            var msg = new MailMessage(from: _options.Email ?? "no-reply@example.com",
                                      to: email,
                                      subject: "Reset Password",
                                      body: resetPasswordMessage)
            {
                IsBodyHtml = true
            };
            await client.SendMailAsync(msg);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to send forgot password email. See inner exception for details.", ex);
        }
    }

    private SmtpClient BuildSmtpClient()
    {
        var host = _options.SmtpServer ?? "smtp.gmail.com";
        var port = _options.SmtpPort > 0 ? _options.SmtpPort : 587;
        var enableSsl = _options.EnableSsl;

        return new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_options.Email, _options.Password)
        };
    }
}





