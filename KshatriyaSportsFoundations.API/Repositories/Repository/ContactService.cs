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
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            KshatriyaSportsFoundationsDbContext dbContext, 
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ContactService> logger)
        {
            _dbContext = dbContext;
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task<EnquiryDomain?> SaveEnquiry(EnquiryDomain sendEnquiryRequest)
        {
            try
            {
                await _dbContext.Enquiries.AddAsync(sendEnquiryRequest);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Enquiry saved successfully for {Name}. Queuing background task...", sendEnquiryRequest.Name);

                // Capture data values to avoid ObjectDisposedException
                var name = sendEnquiryRequest.Name;
                var phone = sendEnquiryRequest.Phone.ToString();
                var email = sendEnquiryRequest.Email;
                var location = sendEnquiryRequest.Location ?? "";
                var message = sendEnquiryRequest.Message ?? "";

                // Queue single background task for both email and WhatsApp
                _taskQueue.QueueBackgroundWorkItem(async cancellationToken =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                    var whatsAppService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<ContactService>>();

                    try
                    {
                        logger.LogInformation("Starting notification tasks for enquiry from {Name}", name);

                        // Send email
                        try
                        {
                            logger.LogInformation("Sending email for enquiry from {Name}", name);
                            await emailSender.SendBulkEmailAsync(
                                emailSender.GetRecipients(), 
                                emailSender.GetSubject(), 
                                emailSender.GetEnquiryEmailContent(name, phone, email, location, message));
                            logger.LogInformation("Email sent successfully for enquiry from {Name}", name);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to send email for enquiry from {Name}: {Message}", name, ex.Message);
                        }

                        // Send WhatsApp
                        try
                        {
                            logger.LogInformation("Sending WhatsApp message for enquiry from {Name}", name);
                            await whatsAppService.SendBulkWhatsAppAsync(
                                whatsAppService.GetRecipients(), 
                                whatsAppService.GetMessage(name, phone, email, location, message));
                            logger.LogInformation("WhatsApp sent successfully for enquiry from {Name}", name);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to send WhatsApp for enquiry from {Name}: {Message}", name, ex.Message);
                        }

                        logger.LogInformation("Notification tasks completed for enquiry from {Name}", name);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error in notification background task for {Name}", name);
                    }
                });

                _logger.LogInformation("Background task queued successfully for {Name}", sendEnquiryRequest.Name);

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
