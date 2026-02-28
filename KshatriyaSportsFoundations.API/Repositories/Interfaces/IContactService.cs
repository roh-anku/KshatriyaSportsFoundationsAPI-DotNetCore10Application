using KshatriyaSportsFoundations.API.Models.Domain;
using KshatriyaSportsFoundations.API.Models.Dtos.Contact;

namespace KshatriyaSportsFoundations.API.Repositories.Interfaces
{
    public interface IContactService
    {
        Task<EnquiryDomain?> SaveEnquiry(EnquiryDomain sendEnquiryRequest);
    }
}
