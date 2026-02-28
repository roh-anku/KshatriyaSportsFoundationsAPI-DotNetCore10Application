namespace KshatriyaSportsFoundations.API.Models.Dtos.Student
{
    public class StudentDetailsResponseDto
    {
        public Guid EnquiryId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long Phone { get; set; }
        public string? Message { get; set; }
        public string? AdminComments { get; set; }
        public bool Fullfilled { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public bool? IsChanged { get; set; }
    }
}
