using KshatriyaSportsFoundations.API.Database;
using KshatriyaSportsFoundations.API.Models.Domain;
using KshatriyaSportsFoundations.API.Models.Dtos.Contact;
using KshatriyaSportsFoundations.API.Repositories.Interfaces;
using KshatriyaSportsFoundations.API.Utilities.BackgroundTasks;
using KshatriyaSportsFoundations.API.Utilities.SendGridEmailSender;
using KshatriyaSportsFoundations.API.Utilities.WhatsappMessageSender;

namespace KshatriyaSportsFoundations.API.Repositories.Repository
{
    public class ContactService : IContactService
    {
        private readonly KshatriyaSportsFoundationsDbContext _dbContext;
        private readonly IEmailSender _emailSender;
        private readonly IWhatsAppService _whatsAppService;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            KshatriyaSportsFoundationsDbContext dbContext, 
            IEmailSender emailSender, 
            IWhatsAppService whatsAppService,
            IBackgroundTaskQueue taskQueue,
            IServiceProvider serviceProvider,
            ILogger<ContactService> logger)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
            _whatsAppService = whatsAppService;
            _taskQueue = taskQueue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<EnquiryDomain?> SaveEnquiry(EnquiryDomain sendEnquiryRequest)
        {
            try
            {
                await _dbContext.Enquiries.AddAsync(sendEnquiryRequest);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Enquiry saved successfully for {Name}. Queuing background tasks...", sendEnquiryRequest.Name);

                // Capture data values to avoid ObjectDisposedException
                var name = sendEnquiryRequest.Name;
                var phone = sendEnquiryRequest.Phone.ToString();
                var email = sendEnquiryRequest.Email;
                var location = "Test location 1";
                var message = sendEnquiryRequest.Message ?? "";

                // Queue background tasks for email and WhatsApp
                _taskQueue.QueueBackgroundWorkItem(async cancellationToken =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<ContactService>>();

                    logger.LogInformation("Starting email send task for enquiry from {Name}", name);

                    await emailSender.SendBulkEmailAsync(
                        emailSender.GetRecipients(), 
                        emailSender.GetSubject(), 
                        emailSender.GetEnquiryEmailContent(name, phone, email, location, message));

                    logger.LogInformation("Email send task completed for enquiry from {Name}", name);
                });

                _taskQueue.QueueBackgroundWorkItem(async cancellationToken =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var whatsAppService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<ContactService>>();

                    logger.LogInformation("Starting WhatsApp send task for enquiry from {Name}", name);

                    await whatsAppService.SendBulkWhatsAppAsync(
                        whatsAppService.GetRecipients(), 
                        whatsAppService.GetMessage(name, phone, email, location, message));

                    logger.LogInformation("WhatsApp send task completed for enquiry from {Name}", name);
                });

                _logger.LogInformation("Background tasks queued successfully for {Name}", sendEnquiryRequest.Name);

                return sendEnquiryRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving enquiry for {Name}: {Message}", sendEnquiryRequest.Name, ex.Message);
                return null;
            }
        }
    }
}
