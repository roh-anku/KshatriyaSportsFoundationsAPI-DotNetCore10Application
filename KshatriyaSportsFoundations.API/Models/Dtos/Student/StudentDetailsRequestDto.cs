namespace KshatriyaSportsFoundations.API.Models.Dtos.Student
{
    public class StudentDetailsRequestDto
    {
        public Guid EnquiryId { get; set; }
        public string? AdminComments { get; set; }
        public bool Fullfilled { get; set; }
        public bool? IsChanged { get; set; }
    }
}
