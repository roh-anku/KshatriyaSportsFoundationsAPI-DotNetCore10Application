using KshatriyaSportsFoundations.API.Database;
using KshatriyaSportsFoundations.API.Models.Domain;
using KshatriyaSportsFoundations.API.Models.Dtos.Contact;
using KshatriyaSportsFoundations.API.Repositories.Interfaces;

namespace KshatriyaSportsFoundations.API.Repositories.Repository
{
    public class ContactService : IContactService
    {
        private readonly KshatriyaSportsFoundationsDbContext _dbContext;
        public ContactService(KshatriyaSportsFoundationsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EnquiryDomain?> SaveEnquiry(EnquiryDomain sendEnquiryRequest)
        {
            try
            {
                await _dbContext.Enquiries.AddAsync(sendEnquiryRequest);
                await _dbContext.SaveChangesAsync();

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
