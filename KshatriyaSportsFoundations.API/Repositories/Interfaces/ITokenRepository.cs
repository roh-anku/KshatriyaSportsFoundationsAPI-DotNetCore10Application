using Microsoft.AspNetCore.Identity;

namespace KshatriyaSportsFoundations.API.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
