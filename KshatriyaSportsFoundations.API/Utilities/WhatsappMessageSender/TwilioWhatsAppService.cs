using Microsoft.Extensions.Options;
using System.Numerics;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;
using Twilio.Types;

namespace KshatriyaSportsFoundations.API.Utilities.WhatsappMessageSender
{
    public class TwilioWhatsAppService : IWhatsAppService
    {
        private readonly TwilioSettings _settings;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TwilioWhatsAppService> _logger;

        public TwilioWhatsAppService(IOptions<TwilioSettings> settings, IConfiguration configuration, ILogger<TwilioWhatsAppService> logger)
        {
            _settings = settings.Value;
            _configuration = configuration;
            _logger = logger;

            // Validate configuration before initializing
            if (string.IsNullOrEmpty(_settings.AccountSid) || string.IsNullOrEmpty(_settings.AuthToken))
            {
                _logger.LogWarning("Twilio configuration is missing or incomplete. WhatsApp messages will not be sent.");
                return;
            }

            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
        }

        public async Task SendWhatsAppAsync(string to, string message)
        {
            await MessageResource.CreateAsync(
                from: new PhoneNumber(_settings.FromNumber),
                to: new PhoneNumber($"whatsapp:{to}"),
                body: message
            );
        }

        public async Task SendBulkWhatsAppAsync(List<string> recipients, string message)
        {
            if (string.IsNullOrEmpty(_settings.AccountSid) || string.IsNullOrEmpty(_settings.AuthToken))
            {
                _logger.LogError("Cannot send WhatsApp messages: Twilio credentials not configured");
                return;
            }

            if (recipients == null || recipients.Count == 0)
            {
                _logger.LogWarning("No recipients found for WhatsApp messages");
                return;
            }

            _logger.LogInformation("Attempting to send WhatsApp messages to {Count} recipients", recipients.Count);

            foreach (var number in recipients)
            {
                try
                {
                    var messageResource = await MessageResource.CreateAsync(
                        from: new PhoneNumber(_settings.FromNumber),
                        to: new PhoneNumber($"whatsapp:{number}"),
                        body: message
                    );

                    _logger.LogInformation("WhatsApp message sent successfully to {Number}. SID: {Sid}, Status: {Status}", 
                        number, messageResource.Sid, messageResource.Status);

                    // Small delay to avoid rate limit
                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send WhatsApp message to {Number}: {Message}", number, ex.Message);
                }
            }
        }

        public List<string> GetRecipients()
        {
            return _configuration.GetSection("WhatsAppTo").Get<List<string>>() ?? new List<string>();
        }

        public string GetMessage(string name,string phone,string email,string location,string message)
        {
            var messageBody = $@"
                                🥋 *New Taekwondo Enquiry Received!*

                                ━━━━━━━━━━━━━━━━━━
                                👤 *Name:* {name}
                                📞 *Phone:* {phone}
                                📧 *Email:* {email}
                                📍 *Location:* {location}
                                ━━━━━━━━━━━━━━━━━━

                                💬 *Message:*
                                {message}

                                ━━━━━━━━━━━━━━━━━━
                                📅 Received On: {DateTime.Now:dd MMM yyyy hh:mm tt}

                                🔥 Please respond to the enquiry as soon as possible.
                                ";
            return messageBody;
        }
    }
}
