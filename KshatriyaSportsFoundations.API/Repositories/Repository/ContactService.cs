using KshatriyaSportsFoundations.API.Database;
using KshatriyaSportsFoundations.API.Models.Domain;
using KshatriyaSportsFoundations.API.Models.Dtos.Contact;
using KshatriyaSportsFoundations.API.Repositories.Interfaces;
using KshatriyaSportsFoundations.API.Utilities.SendGridEmailSender;

namespace KshatriyaSportsFoundations.API.Repositories.Repository
{
    public class ContactService : IContactService
    {
        private readonly KshatriyaSportsFoundationsDbContext _dbContext;
        private readonly IEmailSender _emailSender;
        public ContactService(KshatriyaSportsFoundationsDbContext dbContext, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
        }

        public async Task<EnquiryDomain?> SaveEnquiry(EnquiryDomain sendEnquiryRequest)
        {
            try
            {
                await _dbContext.Enquiries.AddAsync(sendEnquiryRequest);
                await _dbContext.SaveChangesAsync();

                //send email
                await _emailSender.SendBulkEmailAsync(_emailSender.GetRecipients(),_emailSender.GetSubject(),_emailSender.GetEnquiryEmailContent(sendEnquiryRequest.Name,sendEnquiryRequest.Phone.ToString(), sendEnquiryRequest.Email,"Location 1"),sendEnquiryRequest.Message);

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
