namespace KshatriyaSportsFoundations.API.Models.Dtos.Auth
{
    public class LoginResponseDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string JwtToken { get; set; }
    }
}
