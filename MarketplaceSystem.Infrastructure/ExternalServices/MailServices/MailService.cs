using MarketplaceSystem.Application.Common.DTOs.Emails;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IMailServices;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace MarketplaceSystem.Infrastructure.ExternalServices.MailServices
{
    public class MailService : IMailService
    {
        private readonly IConfiguration configuration;
        private readonly string password;
        private readonly string fromEmail;
        public MailService(IConfiguration configuration)
        {
            this.configuration = configuration;
            password = configuration["EmailSettings:Password"];
            fromEmail = configuration["EmailSettings:From"];
        }
        public async Task SendEmailAsync(EmailDto emailDto, CancellationToken cancellation = default)
        {
            MailAddress mailAddress = new MailAddress(fromEmail);
            MailAddress toAddress = new MailAddress(emailDto.To);

            SmtpClient smptClient = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailAddress.Address, password),
                Timeout = 20000
            };

            using var message = new MailMessage(mailAddress, toAddress)
            {
                Subject = emailDto.Subject,
                Body = BodyTemplate(emailDto),
                IsBodyHtml = true
            };

            if (emailDto.FormFile != null)
            {
                using var stream = emailDto.FormFile.OpenReadStream();
                message.Attachments.Add(new Attachment(stream, emailDto.FormFile.FileName));
            }

            try
            {
                await smptClient.SendMailAsync(message, cancellation);
            }
            catch (SmtpFailedRecipientException ex)
            {
                throw new BusinessRuleException($"Email {emailDto.To} không hợp lệ hoặc không thể gửi: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Gửi email thất bại: {ex.Message}");
            }
        }
        public string BodyTemplate(EmailDto emailDto)
        {
            string appName = configuration["AppSettings:AppName"] ?? "Marketplace System";
            string expirationMinutes = "5";

            return $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <title>Xác thực - OTP</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 50px auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }}
        h3 {{
            color: #333333;
        }}
        .otp-code {{
            font-size: 32px;
            font-weight: bold;
            color: #1a73e8;
            letter-spacing: 5px;
            text-align: center;
            margin: 20px 0;
        }}
        p {{
            color: #555555;
        }}
        .footer {{
            margin-top: 30px;
            font-size: 12px;
            color: #999999;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h3>Xin chào,</h3>
        <p>Mã xác thực OTP của bạn cho <strong>chợ tốt</strong> là:</p>
        <div class='otp-code'>{emailDto.Code}</div>
        <p>Vui lòng nhập mã này trong vòng {expirationMinutes} phút.</p>
        <p>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này.</p>
        <div class='footer'>
            &copy; {DateTime.Now.Year} Chợ tốt. All rights reserved.
        </div>
    </div>
</body>
</html>";
        }
    }
}
