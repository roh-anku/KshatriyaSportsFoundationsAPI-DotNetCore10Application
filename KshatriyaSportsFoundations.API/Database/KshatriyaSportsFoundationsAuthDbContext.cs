using KshatriyaSportsFoundations.API.EnumsAndConstants.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KshatriyaSportsFoundations.API.Database
{
    public class KshatriyaSportsFoundationsAuthDbContext : IdentityDbContext
    {
        public KshatriyaSportsFoundationsAuthDbContext(DbContextOptions<KshatriyaSportsFoundationsAuthDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            string userRoleId = "dd76b7e4-4f22-4569-b1e1-dcdc826b746c";
            string adminRoleId = "fa96fd1a-c5c6-4c29-bc3e-9840b0fb079e";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id=userRoleId,
                    ConcurrencyStamp=userRoleId,
                    Name=AuthConstants.User,
                    NormalizedName=AuthConstants.User.ToUpper()
                },
                new IdentityRole
                {
                    Id=adminRoleId,
                    ConcurrencyStamp=adminRoleId,
                    Name=AuthConstants.Admin,
                    NormalizedName=AuthConstants.Admin.ToUpper()
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
