using KshatriyaSportsFoundations.API.Models.Domain;
using KshatriyaSportsFoundations.API.Models.Dtos.Student;

namespace KshatriyaSportsFoundations.API.Repositories.Interfaces
{
    public interface IStudentService
    {
        Task<List<EnquiryDomain>> GetStudentDetails(int pageNo = 1, int pageSize = 1000);
        Task<bool> SaveStudentDetails(List<EnquiryDomain> studentDetails);
        Task<int> GetTotalRecordsCount();
    }
}
