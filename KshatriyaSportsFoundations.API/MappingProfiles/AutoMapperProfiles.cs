using AutoMapper;
using KshatriyaSportsFoundations.API.Models.Domain;
using KshatriyaSportsFoundations.API.Models.Dtos.Contact;
using KshatriyaSportsFoundations.API.Models.Dtos.Student;
namespace KshatriyaSportsFoundations.API.MappingProfiles
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<EnquiryDomain, SendEnquiryRequestDto>().ReverseMap();
            CreateMap<EnquiryDomain, StudentDetailsResponseDto>().ReverseMap();
            CreateMap<StudentDetailsRequestDto, EnquiryDomain>().ReverseMap();
        }
    }
}
