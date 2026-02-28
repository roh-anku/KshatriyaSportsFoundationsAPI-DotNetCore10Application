using System.ComponentModel.DataAnnotations;

namespace KshatriyaSportsFoundations.API.Models.Dtos.Contact
{
    public class SendEnquiryRequestDto
    {
        [Required(ErrorMessage="Name is required")]
        public string Name { get; set; }
        
        [Required(ErrorMessage ="Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Phone is required")]
        [Range(1000000000,9999999999,ErrorMessage ="Please enter valid phone number")]
        public long Phone { get; set; }

        public string? Message { get; set; }
    }
}
