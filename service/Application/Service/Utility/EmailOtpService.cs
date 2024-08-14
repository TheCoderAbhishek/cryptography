using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using service.Core.Enums;
using service.Core.Interfaces.Utility;
using service.Infrastructure.Dependency;

namespace service.Application.Service.Utility
{
    /// <summary>
    /// Class that implements the IEmailOtpService interface for OTP generation and email sending.
    /// </summary>
    public class EmailOtpService(ILogger<EmailOtpService> logger, IOptions<SmtpSettings> smtpSettings) : IEmailOtpService
    {
        private readonly SmtpSettings _smtpSettings = smtpSettings.Value;
        private readonly ILogger<EmailOtpService> _logger = logger;

        /// <summary>
        /// Generates a random OTP (One-Time Password) string of the specified length.
        /// </summary>
        /// <param name="length">Optional length of the OTP. Defaults to 6 characters.</param>
        /// <returns>A string containing the generated OTP.</returns>
        private static string GenerateOtp(int length = 6)
        {
            var random = new Random();
            var otp = new char[length];
            for (int i = 0; i < length; i++)
            {
                otp[i] = (char)('0' + random.Next(0, 10));
            }
            return new string(otp);
        }

        /// <summary>
        /// Asynchronously generates a new One-Time Password.
        /// </summary>
        /// <returns>A task that resolves to a string containing the generated OTP.</returns>
        public string GenerateOtpAsync()
        {
            var otp = GenerateOtp();
            return otp;
        }

        /// <summary>
        /// Asynchronously sends an email containing a One-Time Password (OTP) to the specified recipient.
        /// </summary>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="otp">The One-Time Password to include in the email body.</param>
        /// <param name="subject">The custom email subject.</param>
        /// <param name="body">The custom body content of the email. If not provided, a default message with the OTP code will be used.</param>
        /// <param name="isHtml">HTML email body.</param>
        /// <returns>An asynchronous task representing the email sending operation.</returns>
        public async Task SendOtpEmailAsync(string email, string otp, string subject, string body, bool isHtml = false)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));
            }

            if (string.IsNullOrEmpty(otp))
            {
                throw new ArgumentException($"'{nameof(otp)}' cannot be null or empty.", nameof(otp));
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Ayerhs", _smtpSettings.Username));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            if (string.IsNullOrEmpty(body) || !body.Contains("{otp}"))
            {
                body = ConstantData.GetProfessionalOtpHtmlBody(otp);
                isHtml = true;
            }
            else
            {
                body = body.Replace("{otp}", otp);
            }

            message.Body = new TextPart(isHtml ? "html" : "plain")
            {
                Text = body
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, false);
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("OTP email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to {Email} because {Message}", email, ex.Message);
            }
        }
    }
}
