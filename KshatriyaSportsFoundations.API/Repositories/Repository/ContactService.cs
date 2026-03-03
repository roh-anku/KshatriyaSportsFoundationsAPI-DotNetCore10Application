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

        public ContactService(
            KshatriyaSportsFoundationsDbContext dbContext, 
            IEmailSender emailSender, 
            IWhatsAppService whatsAppService,
            IBackgroundTaskQueue taskQueue,
            IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
            _whatsAppService = whatsAppService;
            _taskQueue = taskQueue;
            _serviceProvider = serviceProvider;
        }

        public async Task<EnquiryDomain?> SaveEnquiry(EnquiryDomain sendEnquiryRequest)
        {
            try
            {
                await _dbContext.Enquiries.AddAsync(sendEnquiryRequest);
                await _dbContext.SaveChangesAsync();

                // Queue background tasks for email and WhatsApp
                _taskQueue.QueueBackgroundWorkItem(async cancellationToken =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                    await emailSender.SendBulkEmailAsync(
                        emailSender.GetRecipients(), 
                        emailSender.GetSubject(), 
                        emailSender.GetEnquiryEmailContent(
                            sendEnquiryRequest.Name, 
                            sendEnquiryRequest.Phone.ToString(), 
                            sendEnquiryRequest.Email, 
                            "Test location 1", 
                            sendEnquiryRequest.Message ?? ""));
                });

                _taskQueue.QueueBackgroundWorkItem(async cancellationToken =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var whatsAppService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();

                    await whatsAppService.SendBulkWhatsAppAsync(
                        whatsAppService.GetRecipients(), 
                        whatsAppService.GetMessage(
                            sendEnquiryRequest.Name, 
                            sendEnquiryRequest.Phone.ToString(), 
                            sendEnquiryRequest.Email, 
                            "Test location 1", 
                            sendEnquiryRequest.Message ?? ""));
                });

                return sendEnquiryRequest;
            }
            catch (Exception ex)
            {
                //log
                return null;
            }
        }
    }
}
