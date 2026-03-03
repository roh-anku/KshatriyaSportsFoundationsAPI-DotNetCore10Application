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

        public TwilioWhatsAppService(IOptions<TwilioSettings> settings, IConfiguration configuration)
        {
            _settings = settings.Value;
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
            _configuration = configuration;
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
            foreach (var number in recipients)
            {
                try
                {
                    await MessageResource.CreateAsync(
                        from: new PhoneNumber(_settings.FromNumber),
                        to: new PhoneNumber($"whatsapp:{number}"),
                        body: message
                    );

                    // Small delay to avoid rate limit
                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                    // Log failure
                    Console.WriteLine($"Failed for {number}: {ex.Message}");
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
