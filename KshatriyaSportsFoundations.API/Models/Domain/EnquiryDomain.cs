using System.ComponentModel.DataAnnotations;

namespace KshatriyaSportsFoundations.API.Models.Domain
{
    public class EnquiryDomain
    {
        [Key]
        public Guid EnquiryId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long Phone { get; set; }
        public string? Message { get; set; }
        public string? AdminComments { get; set; }
        public bool Fullfilled { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }
}
