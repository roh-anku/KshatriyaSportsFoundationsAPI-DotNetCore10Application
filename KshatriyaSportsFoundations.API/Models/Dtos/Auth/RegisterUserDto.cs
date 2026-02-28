using System.ComponentModel.DataAnnotations;

namespace KshatriyaSportsFoundations.API.Models.Dtos.Auth
{
    public class RegisterUserDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string[] Roles { get; set; }
    }
}
