using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Diagnostics;

namespace KshatriyaSportsFoundations.API.Utilities.SendGridEmailSender
{
    public class SendGridEmailService: IEmailSender
    {
        private readonly SendGridSettings _settings;
        private readonly IConfiguration _configuration;
        public SendGridEmailService(IOptions<SendGridSettings> settings,IConfiguration configuration)
        {
            _configuration = configuration;
            _settings = settings.Value;
        }

        public async Task SendBulkEmailAsync(List<string> recipients, string subject, string htmlContent)
        {
            var client = new SendGridClient(_settings.ApiKey);
            var from = new EmailAddress(_settings.FromEmail, _settings.FromName);

            var msg = new SendGridMessage
            {
                From = from,
                Subject = subject,
                HtmlContent = htmlContent
            };

            var tos = recipients.Select(email => new EmailAddress(email)).ToList();
            msg.AddTos(tos);

            await client.SendEmailAsync(msg);
        }

        public List<string> GetRecipients()
        {
            return _configuration.GetSection("EmailsTo").Get<List<string>>() ?? new List<string>();
        }

        public string GetSubject()
        {
            return "Kshatriya Sports Foundations - New Enquiry Received!";
        }

        public string GetEnquiryEmailContent(string name, string phone, string email, string location)
        {
            return $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>New Enquiry Received</title>
                    </head>
                    <body style='margin:0;padding:0;background-color:#f4f6f9;font-family:Arial,Helvetica,sans-serif;'>

                        <table align='center' width='100%' cellpadding='0' cellspacing='0' 
                               style='max-width:600px;margin:30px auto;background:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 4px 10px rgba(0,0,0,0.08);'>
        
                            <!-- Header -->
                            <tr>
                                <td style='background-color:#1e293b;padding:20px;text-align:center;color:#ffffff;font-size:20px;font-weight:bold;'>
                                    📩 New Enquiry Received
                                </td>
                            </tr>

                            <!-- Body -->
                            <tr>
                                <td style='padding:25px;'>

                                    <p style='font-size:16px;color:#333;'>Hello Admin,</p>

                                    <p style='font-size:15px;color:#555;'>
                                        You have received a new enquiry with the following details:
                                    </p>

                                    <table width='100%' cellpadding='10' cellspacing='0' 
                                           style='margin-top:15px;border-collapse:collapse;font-size:14px;'>
                    
                                        <tr style='background-color:#f8fafc;'>
                                            <td style='font-weight:bold;width:35%;border:1px solid #e5e7eb;'>Name</td>
                                            <td style='border:1px solid #e5e7eb;'>{name}</td>
                                        </tr>

                                        <tr>
                                            <td style='font-weight:bold;border:1px solid #e5e7eb;'>Phone Number</td>
                                            <td style='border:1px solid #e5e7eb;'>
                                                <a href='tel:{phone}' style='color:#2563eb;text-decoration:none;'>{phone}</a>
                                            </td>
                                        </tr>

                                        <tr style='background-color:#f8fafc;'>
                                            <td style='font-weight:bold;border:1px solid #e5e7eb;'>Email</td>
                                            <td style='border:1px solid #e5e7eb;'>
                                                <a href='mailto:{email}' style='color:#2563eb;text-decoration:none;'>{email}</a>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style='font-weight:bold;border:1px solid #e5e7eb;'>Location</td>
                                            <td style='border:1px solid #e5e7eb;'>{location}</td>
                                        </tr>

                                    </table>

                                    <p style='margin-top:25px;font-size:14px;color:#777;'>
                                        Please respond to the enquiry at the earliest.
                                    </p>

                                </td>
                            </tr>

                            <!-- Footer -->
                            <tr>
                                <td style='background-color:#f1f5f9;padding:15px;text-align:center;font-size:12px;color:#6b7280;'>
                                    This is an automated notification from your website.
                                </td>
                            </tr>

                        </table>

                    </body>
                    </html>";
        }
    }
}
